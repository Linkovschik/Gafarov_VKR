using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gafarov_VKR.Models.AlgorithmClass
{
    public class Algorithm
    {
        public List<Path> AllSolutions { get; set; }
        public double _Literally_Avg_For_10_Rides_ { get; set; }
        public double _Literally_Max_For_10_Rides_ { get; set; }


        private double AlgorithmTime { get; set; }
        public Dictionary<string, double> SignPenalties { get; set; }
        public Dictionary<string, double> ManeuverPenalties { get; set; }

        private double AverageCost { get; set; }
        private double Cost { get; set; }
        private List<BaseMark> ResultMarks { get; set; }

        private Dictionary<string, int> SignProblems { get; set; }
        private Dictionary<string, int> ManeuverProblems { get; set; }
        private Point StartPoint { get; set; }
        private List<Sign> Signs { get; set; }
        private List<Maneuver> Maneuvers { get; set; }
        //средняя скорость в метрах в минуту
        public double AverageSpeed { get; set; }
        //время на заезд в минутах
        public double Time { get; set; }

        //averageSpeed - м/минуту
        public Algorithm(
            Dictionary<string,int> signProblems, 
            Dictionary<string, int> maneuverProblems,
            Point startPoint,
            List<Sign> signs,
            List<Maneuver> maneuvers,
            int time,
            int speed)
        {
            //средняя скорость = 500 м/мин (30 км/ч) по умолчанию 
            AverageSpeed = speed * 1000.0 / 60.0;
            //время в минутах
            Time = time;
            SignProblems = signProblems;
            ManeuverProblems = maneuverProblems;
            StartPoint = startPoint;
            Signs = signs;
            Maneuvers = maneuvers;

            SignPenalties = new Dictionary<string, double>()
            {
                {"Пешеходный переход", 0 },
                {"Знак СТОП", 0 },
                {"Ограничение скорости", 0 }
            };
            ManeuverPenalties = new Dictionary<string, double>()
            {
                {"Разворот", 0 },
                {"Поворот налево", 0 },
                {"Поворот направо", 0 }
            };

            var keys = SignProblems.Keys.ToList();
            foreach (var key in keys)
            {
                int current = SignProblems[key];
                SignProblems[key] = current + 1;
            }

            keys = ManeuverProblems.Keys.ToList();
            foreach (var key in keys)
            {
                int current = ManeuverProblems[key];
                ManeuverProblems[key] = current + 1;
            }

            _Literally_Avg_For_10_Rides_ = 0;
            _Literally_Max_For_10_Rides_ = 0;
            AllSolutions = new List<Path>();
        }

        public List<BaseMark> GetResultingMarks()
        {
            return ResultMarks;
        }

        public double GetCost()
        {
            return _Literally_Max_For_10_Rides_;
        }

        public double GetAverageCost()
        {
            return _Literally_Avg_For_10_Rides_;
        }

        public double GetAlgorithmTime()
        {
            return AlgorithmTime;
        }


        public void ExecuteAlgorithm()
        {
            int TRYCOUNT = 1;
            for (int replayCount = 0; replayCount < TRYCOUNT; replayCount++)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                List<BaseMark> result = new List<BaseMark>();
                Random random = new Random();
                const int ITERATIONCOUNT = 100;
                const int PATHCOUNT = 10;
                //const int MUTATIONCOUNT = 5;
                const int CHILDCOUNT = 50;

                if(PATHCOUNT<2)
                {
                    throw new Exception();
                }

                List<BaseMark> allMarks = new List<BaseMark>();
                allMarks.AddRange(Signs);
                allMarks.AddRange(Maneuvers);

                BaseMark startMarker = new BaseMark()
                {
                    Id = 0,
                    AverageDifficulty = 0,
                    StartMarkerPoint = StartPoint
                };

                List<Path> pathList = Path.GeneratePaths(random, PATHCOUNT, allMarks, startMarker, SignProblems, ManeuverProblems, SignPenalties, ManeuverPenalties, AverageSpeed, Time);

                for (int iter = 0; iter < ITERATIONCOUNT; ++iter)
                {
                    Dictionary<double, Path> allDescendants = new Dictionary<double, Path>();
                    foreach (Path path in pathList)
                    {
                        //родители участвуют в отборе
                        if (!allDescendants.Keys.Contains(path.Time * path.Cost))
                            allDescendants.Add(path.Time * path.Cost, path);
                    }

                    for (int crossoverCount = 0; crossoverCount < CHILDCOUNT; ++crossoverCount)
                    {
                        int motherIndex = random.Next(0, pathList.Count);
                        int fatherIndex = random.Next(0, pathList.Count);
                        int tries = 0;
                        while (fatherIndex == motherIndex && tries<100)
                        {
                            fatherIndex = random.Next(0, pathList.Count);
                            tries += 1;
                        }
                        //если так и не получилось взять разных отца и мать
                        if (fatherIndex == motherIndex)
                        {
                            fatherIndex = motherIndex > 0 ? motherIndex - 1 : motherIndex + 1;
                        }

                        Path child = pathList[motherIndex].Crossover(random, pathList[fatherIndex]);
                        child.Mutate(random, Time);
                        if (!allDescendants.Keys.Contains(child.Time * child.Cost))
                            allDescendants.Add(child.Time * child.Cost, child);
                    }

                    for (int mutatantIndex = 0; mutatantIndex < PATHCOUNT; ++mutatantIndex)
                    {
                        int mutantIndex = random.Next(0, pathList.Count);
                        Path mutant = Path.GetCopy(pathList[mutantIndex]);
                        mutant.Mutate(random, Time);
                        if (!allDescendants.Keys.Contains(mutant.Time * mutant.Cost))
                            allDescendants.Add(mutant.Time * mutant.Cost, mutant);
                    }

                    List<Path> values = new List<Path>();
                    foreach (var t in allDescendants)
                    {
                        values.Add(t.Value);
                    }
                    values = values.Where(p => p.Time <= Time).OrderByDescending(p => p.Cost).ToList();
                    if (values.Count >= PATHCOUNT)
                        pathList = values.GetRange(0, PATHCOUNT);
                    else
                        pathList = values.GetRange(0, values.Count);
                }

                foreach (var mark in pathList[0].InputMarkList)
                {
                    result.Add(mark);
                }

                ResultMarks = result;
                Cost = pathList[0].Cost;
                AverageCost = pathList[0].AverageCost;
                watch.Stop();

                AlgorithmTime += (watch.ElapsedMilliseconds/1000.00);
                _Literally_Max_For_10_Rides_ += Cost;
                //_Literally_Avg_For_10_Rides_ += Cost;
                AllSolutions.Add(pathList[0]);
            }

            if(AllSolutions!=null)
            {
                for(int i=0; i<AllSolutions.Count; ++i)
                {
                    double diff = 0;
                    for (int j = 0; j < AllSolutions.Count; ++j)
                    {
                        if (i != j)
                        {
                            diff +=
                                CountDifferenceOfSolutions(AllSolutions[i], AllSolutions[j]);
                        }
                    }
                    diff /= AllSolutions.Count;
                    _Literally_Avg_For_10_Rides_ += diff;
                }
            }
            _Literally_Avg_For_10_Rides_ /= (double)TRYCOUNT;
            _Literally_Max_For_10_Rides_ /= (double)TRYCOUNT;
            AlgorithmTime /= (double)TRYCOUNT;
        }

        public int CountDifferenceOfSolutions(Path p1, Path p2)
        {
            int result = 0;
            foreach (var mark in p1.InputMarkList)
            {
                if (mark is Sign &&
                    !p2.InputMarkList.Any(x => ((x is Sign) && x.Id == mark.Id)))
                {
                    result += 1;
                }
                if (mark is Maneuver &&
                    !p2.InputMarkList.Any(x => ((x is Maneuver) && x.Id == mark.Id)))
                {
                    result += 1;
                }
            }
            return result;
        } 
    }
}