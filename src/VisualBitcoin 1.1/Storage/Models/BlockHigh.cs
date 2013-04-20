using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage.Models
{
    public class BlockHigh
    {
        // Properties.
		public string Hash { get; set; }
		public int Time { get; set; }


		// Constructors.
		public BlockHigh()
		{
			Hash = "default";
			Time = 0;
		}

		public BlockHigh(string hash, int time)
		{
			Hash = hash;
			Time = time;
		}

    }
}
