using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Interpolation = MathNet.Numerics.Interpolation;

namespace Gafarov_VKR.Models.AlgorithmClass
{
    public class Path
    {
        public static double SortFuction(Interpolation.LinearSpline interpolation, Point point, Point startPoint)
        {
            
            double result = interpolation.Interpolate(point.Lat)- interpolation.Interpolate(startPoint.Lat);
            return result;
        }
        public static List<Path> GeneratePaths(
            Random random, 
            int pathCount,
            List<BaseMark> _allMarks, 
            BaseMark startMarker, 
            Dictionary<string, int>  signProblems, 
            Dictionary<string, int> maneuverProblems,
            Dictionary<string, double> signPenalties,
            Dictionary<string, double> maneuverPenalties,
            double speed, 
            double time)
        {
            List<double> Lats = new List<double>();
            List<double> Lngs = new List<double>();
            Lats.Add(startMarker.StartMarkerPoint.Lat);
            Lngs.Add(startMarker.StartMarkerPoint.Lng);
            foreach(var mark in _allMarks)
            {
                Lats.Add(mark.StartMarkerPoint.Lat);
                Lngs.Add(mark.StartMarkerPoint.Lng);
            }
            Interpolation.LinearSpline interpolation = Interpolation.LinearSpline.Interpolate(Lats, Lngs);
            var allMarks = _allMarks.OrderBy(mark => Path.SortFuction(interpolation,mark.StartMarkerPoint,startMarker.StartMarkerPoint)).ToList();


            List<Path> result = new List<Path>();
            for(int i=0; i< pathCount; ++i)
            {
                Path path = new Path(allMarks, startMarker, signProblems, maneuverProblems, signPenalties, maneuverPenalties, speed);
                int index = 0;
                while (true)
                {
                    index += random.Next(0, 5);
                    index %= allMarks.Count;
                    path.AddMarker(allMarks[index]);
                    if(path.Time > time)
                    {
                        path.RemoveLastMarker();
                        break;
                    }
                }
                result.Add(path);
            }
            
            return result;
        }
        public static Path GetCopy(Path pathToCopy)
        {
            Path path = new Path();
            foreach(var pair in pathToCopy.SignCountsById)
            {
                path.SignCountsById.Add(pair.Key, pair.Value);
            }
            foreach (var pair in pathToCopy.ManeuverCountsById)
            {
                path.ManeuverCountsById.Add(pair.Key, pair.Value);
            }
            foreach(var mark in pathToCopy.InputMarkList)
            {
                path.InputMarkList.Add(mark);
            }
            path.AllKnownMarks = pathToCopy.AllKnownMarks;
            path.StartMarker = pathToCopy.StartMarker;
            path.SignProblems = pathToCopy.SignProblems;
            path.ManeuverProblems = pathToCopy.ManeuverProblems;
            path.SignPenalties = pathToCopy.SignPenalties;
            path.ManeuverPenalties = pathToCopy.ManeuverPenalties;
            path.Speed = pathToCopy.Speed;
            path.Time = pathToCopy.Time;
            path.Cost = pathToCopy.Cost;
            path.AverageCost = pathToCopy.AverageCost;

            return path;
        }

        //штрафы за знаки, тип:минуты
        private Dictionary<string, double> SignPenalties { get; set; }

        //штрафы за знаки, тип:минуты
        private Dictionary<string, double> ManeuverPenalties { get; set; }

        private Dictionary<int,int> SignCountsById { get; set; }
        private Dictionary<int, int> ManeuverCountsById { get; set; }
        public List<BaseMark> InputMarkList { get; set; }
        private List<BaseMark> AllKnownMarks { get; set; }
        private BaseMark StartMarker { get; set; }
        private Dictionary<string, int> SignProblems { get; set; }
        private Dictionary<string, int> ManeuverProblems { get; set; }

        //скорость в метрах/минуту
        private double Speed { get; set; }
        //время в минутах
        public double Time { get; set; }

        //ценность в у.е. (уровня сложности)
        public double Cost { get; set; }

        //средняя ценность решения
        public double AverageCost { get; set; }
        

        private Path()
        {
            InputMarkList = new List<BaseMark>();
            SignCountsById = new Dictionary<int, int>();
            ManeuverCountsById = new Dictionary<int, int>();
        }
        private Path(
            List<BaseMark> allMarks,
            BaseMark startMarker,
            Dictionary<string, int> signProblems,
            Dictionary<string, int> maneuverProblems,
            Dictionary<string, double> signPenalties,
            Dictionary<string, double> maneuverPenalties,
            double speed)
        {
            
            AllKnownMarks = allMarks;
            StartMarker = startMarker;
            SignProblems = signProblems;
            ManeuverProblems = maneuverProblems;
            SignPenalties = signPenalties;
            ManeuverPenalties = maneuverPenalties;

            InputMarkList = new List<BaseMark>();
            InputMarkList.Add(StartMarker);
            InputMarkList.Add(StartMarker);

            Speed = speed;
            SignCountsById = new Dictionary<int, int>();
            ManeuverCountsById = new Dictionary<int, int>();
            countCostAndTime();
        }
        public void AddMarker(BaseMark mark)
        {
            InputMarkList.RemoveAt(InputMarkList.Count - 1);
            InputMarkList.Add(mark);
            InputMarkList.Add(StartMarker);
            countCostAndTime();
        }

        public void RemoveLastMarker()
        {
            InputMarkList.RemoveAt(InputMarkList.Count - 2);
            countCostAndTime();
        }
        //обновить стоимость и время маршрута
        private void countCostAndTime()
        {
            SignCountsById.Clear();
            ManeuverCountsById.Clear();
            //считаем количество конкретного знака или манёвра в подборке
            foreach (var mark in InputMarkList)
            {
                if (mark is Sign)
                {
                    if (SignCountsById.ContainsKey(mark.Id))
                    {
                        SignCountsById[mark.Id] += 1;
                    }
                    else
                    {
                        SignCountsById.Add(mark.Id, 0);
                    }
                }
                else if (mark is Maneuver)
                {
                    if (ManeuverCountsById.ContainsKey(mark.Id))
                    {
                        ManeuverCountsById[mark.Id] += 1;
                    }
                    else
                    {
                        ManeuverCountsById.Add(mark.Id, 0);
                    }
                }
            }

            Cost = 0;
            AverageCost = 0;
            Time = 0;
            double distance = 0;
            Point currentPoint = StartMarker.StartMarkerPoint;
            Vector previousVector = new Vector(currentPoint, currentPoint);
            foreach(var mark in InputMarkList)
            {
                Point nextPoint = null;
                //подсчёт времени
                if (mark is Sign)
                {
                    var sign = (mark as Sign);
                    distance += Point.CalculatePseudoManhattanDistance(currentPoint, sign.StartMarkerPoint);
                    if (Double.IsNaN(distance))
                    {
                        ;
                    }
                    //расчёт штрафа
                    double penalty = 0;
                    penalty += SignPenalties[sign.SignTypeName] * Speed;
                    distance += penalty;
                    if (Double.IsNaN(distance))
                    {
                        ;
                    }
                    //расчёт штрафа за смену направления
                    double directionPenalty = 0;
                    Vector v1 = new Vector(currentPoint, sign.StartMarkerPoint);
                    directionPenalty+= (1 - Vector.GetCOS(v1, previousVector) - 1) * ManeuverPenalties["Разворот"] * Speed;
                    distance += directionPenalty;
                    if (Double.IsNaN(distance))
                    {
                        ;
                    }
                    nextPoint = (mark as Sign).StartMarkerPoint;
                }
                else if(mark is Maneuver)
                {
                    var maneuver = (mark as Maneuver);
                    distance += Point.CalculatePseudoManhattanDistance(currentPoint, maneuver.StartMarkerPoint);
                        
                    if(maneuver.MediumMarkerPoint!=null)
                    {
                        distance += Point.CalculatePseudoManhattanDistance(maneuver.StartMarkerPoint, maneuver.MediumMarkerPoint);
                        distance += Point.CalculatePseudoManhattanDistance(maneuver.MediumMarkerPoint, maneuver.EndMarkerPoint);
                    }
                    else
                    {
                        distance += Point.CalculatePseudoManhattanDistance(maneuver.StartMarkerPoint, maneuver.EndMarkerPoint);
                    }

                    //расчёт штрафа
                    double penalty = 0;
                    int penaltyCount = 1;
                    Vector v1 = new Vector(currentPoint, maneuver.StartMarkerPoint);
                    if (maneuver.ManeuverTypeName == "Разворот")
                    {
                           
                        Vector p_v1 = v1.GetRightPerpendicular();
                        Vector v2 = new Vector(maneuver.StartMarkerPoint, maneuver.EndMarkerPoint);
                        if (Vector.GetCOS(p_v1, v2) > 0)
                        {
                            penaltyCount +=1;
                        }
                    }
                    penalty += penaltyCount * ManeuverPenalties[maneuver.ManeuverTypeName] * Speed;
                    distance += penalty;

                    //расчёт штрафа за смену направления
                    double directionPenalty = 0;
                    directionPenalty += (1 - Vector.GetCOS(v1, previousVector) - 1) * ManeuverPenalties["Разворот"] * Speed;
                    distance += directionPenalty;

                    nextPoint = (mark as Maneuver).EndMarkerPoint;
                }

                if (nextPoint != null)
                {
                    previousVector = new Vector(currentPoint, nextPoint);
                    currentPoint = nextPoint;
                }

                double averageSignProblem = SignProblems.Values.Average();
                double averageManeuversProblem = ManeuverProblems.Values.Average();

                //подсчёт стоимости
                if (mark is Sign)
                {
                    double signProblemProportion = 1;
                    if (averageSignProblem > 0)
                        signProblemProportion = SignProblems[(mark as Sign).SignTypeName] / averageSignProblem;
                    Cost += mark.AverageDifficulty * Math.Pow(0.5, SignCountsById[mark.Id]) * signProblemProportion;
                }
                else if(mark is Maneuver)
                {
                    double maneuverProblemProportion = 1;
                    if (averageManeuversProblem > 0)
                        maneuverProblemProportion = ManeuverProblems[(mark as Maneuver).ManeuverTypeName] / averageManeuversProblem;
                    Cost += mark.AverageDifficulty * Math.Pow(0.7, ManeuverCountsById[mark.Id]) * maneuverProblemProportion;
                }

            }
            
            Time = distance / Speed;
            AverageCost = Cost / InputMarkList.Count;
        }

        private void removingMutation(int averageMutationLength, Random random)
        {
            if (InputMarkList.Count < 3)
                return;
            //не включаем точку начала и точку конца
            int startIndex = random.Next(1, InputMarkList.Count - 1);
            int deleteCount = random.Next(1, averageMutationLength);
            int endIndex = startIndex + deleteCount - 1;
            if (endIndex < InputMarkList.Count - 1)
            {
                InputMarkList.RemoveRange(startIndex, deleteCount);
            }
            else
            {
                InputMarkList.RemoveRange(startIndex, InputMarkList.Count - 1 - startIndex);
            }
            if(InputMarkList[0].StartMarkerPoint.Lat != InputMarkList[InputMarkList.Count-1].StartMarkerPoint.Lat &&
                InputMarkList[0].StartMarkerPoint.Lng != InputMarkList[InputMarkList.Count - 1].StartMarkerPoint.Lng)
            {
                throw new Exception("Мутация удаления отработала с ошибкой!");
            }
        }

        private void addingMutation(int averageMutationLength, Random random)
        {
            if (InputMarkList.Count < 3)
                return;
            int startIndex = random.Next(1, InputMarkList.Count - 1);
            int addCount = random.Next(1, averageMutationLength);
            int indexOfStart = AllKnownMarks.IndexOf(InputMarkList[startIndex]);
            int allMarksStartIndex = (indexOfStart - addCount / 2) >= 1 ?
                (indexOfStart - addCount / 2) : 1;
            int allMarksCount = (allMarksStartIndex + addCount - 1) < AllKnownMarks.Count ?
                addCount : AllKnownMarks.Count - allMarksStartIndex;

            double addProbability = 0;

            for (int i = allMarksStartIndex; i < allMarksStartIndex + allMarksCount; ++i)
            {
                addProbability = random.NextDouble();
                if (addProbability > 0.0 && addProbability < 0.7)
                {
                    InputMarkList.RemoveAt(InputMarkList.Count - 1);
                    InputMarkList.Add(AllKnownMarks[i]);
                    InputMarkList.Add(StartMarker);
                }
            }
            if (InputMarkList[0].StartMarkerPoint.Lat != InputMarkList[InputMarkList.Count - 1].StartMarkerPoint.Lat &&
               InputMarkList[0].StartMarkerPoint.Lng != InputMarkList[InputMarkList.Count - 1].StartMarkerPoint.Lng)
            {
                throw new Exception("Мутация добавления отработала с ошибкой!");
            }
        }

        private void changingMutation(int averageMutationLength, Random random)
        {
            if (InputMarkList.Count < 3)
                return;
            int changeCount = random.Next(1, averageMutationLength);

            int onChangeIndex = 0;
            int chosenIndex = random.Next(1, InputMarkList.Count - 1);
            for(int i=0; i<changeCount; ++i)
            {
                int onChangeStart = (chosenIndex - changeCount / 2) >= 1 ?
                                      (chosenIndex - changeCount / 2) : 1;
                int onChangeEnd = (onChangeStart + changeCount - 1) < InputMarkList.Count - 1 ?
                                                        onChangeStart + changeCount : 
                                                        InputMarkList.Count - 1;
                onChangeIndex = random.Next(onChangeStart, onChangeEnd);
                var plate = InputMarkList[chosenIndex];
                InputMarkList[chosenIndex] = InputMarkList[onChangeIndex];
                InputMarkList[onChangeIndex] = plate;
            }
            if (InputMarkList[0].StartMarkerPoint.Lat != InputMarkList[InputMarkList.Count - 1].StartMarkerPoint.Lat &&
               InputMarkList[0].StartMarkerPoint.Lng != InputMarkList[InputMarkList.Count - 1].StartMarkerPoint.Lng)
            {
                throw new Exception("Мутация изменения отработала с ошибкой!");
            }
        }

        private void changingGroupMutation(int averageMutationLength, Random random)
        {
            if (InputMarkList.Count < 3)
                return;
            int startIndex = random.Next(1, InputMarkList.Count - 1);
            int changeCount = random.Next(1, averageMutationLength);
            int endIndex = startIndex + changeCount - 1;
            List<BaseMark> toChangePlace;
            if (endIndex < InputMarkList.Count - 1)
            {
                toChangePlace = InputMarkList.GetRange(startIndex, changeCount);
                InputMarkList.RemoveRange(startIndex, changeCount);
            }
            else
            {
                toChangePlace = InputMarkList.GetRange(startIndex, InputMarkList.Count - 1 - startIndex);
                InputMarkList.RemoveRange(startIndex, InputMarkList.Count - 1 - startIndex);
            }
            int newPlace = random.Next(1, InputMarkList.Count - 1);
            InputMarkList.InsertRange(newPlace, toChangePlace);
        }

        private void lessLimitMutation(Random random)
        {
            if (InputMarkList.Count < 3)
                return;
            int averageMutationLength = random.Next(1, 5);

            double probability = random.NextDouble();
            //удаление случайной метки или группы меток
            if (probability >= 0.0 && probability < 0.2)
            {
                removingMutation(averageMutationLength, random);
            }
            //добавление случайной метки или группы меток
            else if (probability >= 0.2 && probability < 0.8)
            {
                addingMutation(averageMutationLength, random);
            }
            //поменять метки или группы меток местами
            else if (probability >= 0.8 && probability <= 1)
            {
                changingMutation(averageMutationLength, random);
            }
        }

        private void onLimitMutation(Random random)
        {
            if (InputMarkList.Count < 3)
                return;
            int averageMutationLength = random.Next(1, 5);

            double probability = random.NextDouble();
            //удаление случайной метки или группы меток
            if (probability >= 0.0 && probability < 0.4)
            {
                removingMutation(averageMutationLength, random);
            }
            //добавление случайной метки или группы меток
            else if (probability >= 0.4 && probability < 0.7)
            {
                addingMutation(averageMutationLength, random);
            }
            //поменять метки или группы меток местами
            else if (probability >= 0.7 && probability <= 1)
            {
                changingMutation(averageMutationLength, random);
            }
        }

        private void moreLimitMutation(Random random)
        {
            if (InputMarkList.Count < 3)
                return;
            int averageMutationLength = random.Next(1, 5);

            double probability = random.NextDouble();
            //удаление случайной метки или группы меток
            if (probability >= 0.0 && probability < 0.5)
            {
                removingMutation(averageMutationLength, random);
            }
            //добавление случайной метки или группы меток
            else if (probability >= 0.5 && probability < 0.6)
            {
                addingMutation(averageMutationLength, random);
            }
            //поменять метки или группы меток местами
            else if (probability >= 0.6 && probability <= 1)
            {
                changingMutation(averageMutationLength, random);
            }
        }


        public void Mutate(Random random, double time)
        {

            if (Time > time)
            {
                moreLimitMutation(random);
            }
            else if (Time <= 0.9 * time)
            {
                lessLimitMutation(random);
            }
            else if (Time > 0.9 * time && Time <= time)
            {
                onLimitMutation(random);
            }
            //обязательно пересчитать время и стоимость после изменений!
            countCostAndTime();
        }
    }


}