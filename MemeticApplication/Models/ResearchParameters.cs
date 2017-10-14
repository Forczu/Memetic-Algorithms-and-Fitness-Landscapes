using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MemeticApplication.Models
{
    public class ResearchParameters
    {
        public float Sensitivity { get; set; }

        public int RandomWalkNumber { get; set; }

        public int RandomWalkSteps { get; set; }

        public string[] CrossoverOperators { get; set; }

        public string[] MutationOperators { get; set; }

        public int RoadWeight200 { get; set; }

        public int RoadWeight400 { get; set; }

        public int RoadWeight600 { get; set; }

        public int RoadWeight800 { get; set; }

        public int RoadWeight1000 { get; set; }
    }
}