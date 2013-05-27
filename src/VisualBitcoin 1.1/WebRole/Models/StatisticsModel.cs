using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Storage.Models;

namespace WebRole.Models
{
    public class StatisticsModel 
    {
        //
        [Display(Name = "Number of Blocks")]
        public ulong NumberOfBlocks { get; set; }
        // Time.
        [Display(Name = "à compléter")]
        public ulong TotalTime { get; set; }
        [Display(Name = "à compléter")]
        public double AverageTime { get; set; }
        [Display(Name = "à compléter")]
        public double VarianceTime { get; set; }
        [Display(Name = "à compléter")]
        public double StandardDeviationTime { get; set; }

        // Statistics blocks BTC.
        [Display(Name = "à compléter")]
        public ulong SumBtc { get; set; }
        [Display(Name = "à compléter")]
        public double AverageBtc { get; set; }
        [Display(Name = "à compléter")]
        public double StandardDeviationBtc { get; set; }
        [Display(Name = "à compléter")]
        public double VarianceBtc { get; set; }

        // Statistics blocks transactions.
        [Display(Name = "à compléter")]
        public ulong NumberOfTransactions { get; set; }
        [Display(Name = "à compléter")]
        public double AverageTrans { get; set; }
        [Display(Name = "à compléter")]
        public double VarianceTrans { get; set; }
        [Display(Name = "à compléter")]
        public double StandardDevTrans { get; set; }

        public StatisticsModel(Statistics stats)
        {
            TotalTime = stats.TotalTime;
            AverageTime = stats.AverageTime;
            VarianceTime = stats.VarianceTime;
            StandardDeviationTime = stats.StandardDeviationTime;
            SumBtc = stats.SumBtc;
            AverageBtc = stats.AverageBtc;
            StandardDeviationBtc = stats.StandardDeviationBtc;
            VarianceBtc = stats.VarianceBtc;
            NumberOfTransactions = stats.NumberOfTransactions;
            AverageTrans = stats.AverageTrans;
            VarianceTrans = stats.VarianceTrans;
            StandardDevTrans = stats.StandardDevTrans;
        }

    }
}
