﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.Model
{
    class JsonFormat
    {
        public string writing_time { get; set; }
        public int eligible_files_nbr { get; set; }
        public string total_size_backup { get; set; }
        public int backup_progress { get; set; }
        public string left_files_size { get; set; }
        public string current_backup_file { get; set; }
    }
}
