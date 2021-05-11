using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gafarov_VKR.Models.AlgorithmClass
{
    public class Algorithm
    {
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
            //средняя скорость = 500 метров/минуту по умолчанию
            AverageSpeed = speed * 1000.0 / 60.0;
            Time = time;
            SignProblems = signProblems;
            ManeuverProblems = maneuverProblems;
            StartPoint = startPoint;
            Signs = signs;
            Maneuvers = maneuvers;
        }

        public List<BaseMark> GetResultingMarks()
        {
            List<BaseMark> result = new List<BaseMark>();
            Random random = new Random();
            const int ITERATIONCOUNT = 1000;
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

            List<Path> pathList = Path.GeneratePaths(random, PATHCOUNT, allMarks, startMarker, SignProblems, ManeuverProblems, AverageSpeed, Time);

            for (int iter = 0; iter < ITERATIONCOUNT; ++iter)
            {
                Dictionary<double, Path> allDescendants = new Dictionary<double, Path>();
                foreach(Path path in pathList)
                {
                    if (!allDescendants.Keys.Contains(path.Time * path.Cost))
                        allDescendants.Add(path.Time * path.Cost, path);
                    for(int mutatantIndex=0; mutatantIndex < MUTATIONCOUNT; ++mutatantIndex)
                    {
                        Path mutant = Path.GetCopy(path);
                        mutant.Mutate(random, Time);
                        if (!allDescendants.Keys.Contains(mutant.Time * mutant.Cost))
                            allDescendants.Add(mutant.Time * mutant.Cost, mutant);
                    }
                }
                List<Path> values = new List<Path>();
                foreach(var t in allDescendants)
                {
                    values.Add(t.Value);
                }
                values = values.Where(p => p.Time <= Time).OrderByDescending(p=>p.Cost).ToList();
                pathList = values.GetRange(0, PATHCOUNT);
            }

            foreach(var mark in pathList[0].InputMarkList)
            {
                result.Add(mark);
            }
            return result;
        }
    }
}