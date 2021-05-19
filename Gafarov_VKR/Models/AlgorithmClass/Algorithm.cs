using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gafarov_VKR.Models.AlgorithmClass
{
    public class Algorithm
    {
        public double _Literally_Avg_For_10_Rides_ { get; set; }
        public double _Literally_Max_For_10_Rides_ { get; set; }

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

        public void ExecuteAlgorithm()
        {
            for (int replayCount = 0; replayCount < 10; replayCount++)
            {

                List<BaseMark> result = new List<BaseMark>();
                Random random = new Random();
                const int ITERATIONCOUNT = 100;
                const int PATHCOUNT = 10;
                const int MUTATIONCOUNT = 5;

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
                        if (!allDescendants.Keys.Contains(path.Time * path.Cost))
                            allDescendants.Add(path.Time * path.Cost, path);
                        for (int mutatantIndex = 0; mutatantIndex < MUTATIONCOUNT; ++mutatantIndex)
                        {
                            Path mutant = Path.GetCopy(path);
                            mutant.Mutate(random, Time);
                            if (!allDescendants.Keys.Contains(mutant.Time * mutant.Cost))
                                allDescendants.Add(mutant.Time * mutant.Cost, mutant);
                        }
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
                _Literally_Max_For_10_Rides_ = Math.Max(_Literally_Max_For_10_Rides_, Cost);
                _Literally_Avg_For_10_Rides_ += Cost;
            }
            _Literally_Avg_For_10_Rides_ /= 10;
        }
    }
}