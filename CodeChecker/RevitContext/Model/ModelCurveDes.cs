using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeChecker.RevitContext.Model
{
   public class ModelCurveDes
   {
      public ModelCurveDes(string id, string levelName, double length)
      {
         Id = id;
         LevelName = levelName;
         Length = length;
      }

      public string Id { get; set; }
      public string LevelName { get; set; }
      public double Length { get; set; }
   }
}
