using System;
using System.Collections.Generic;
using System.Text;

namespace NU_Solver.DAO
{
    class file_list
    {
        public string FileName { get; set; }
        public string SolverName { get; set; }
        public string SolveStatus { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string PaperCode { get; set; }
        public string UploadedDate { get; set; }
        public int NoofRows { get; set; }
        public int FileSize { get; set; }
        public string SpanStatus { get; set; }
    }
}
