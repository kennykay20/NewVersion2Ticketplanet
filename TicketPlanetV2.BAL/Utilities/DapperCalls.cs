using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketPlanetV2.BAL.Utilities
{
   public  class DapperCalls
    {
       IDbConnection db = null;

       public DapperCalls()
       {
           db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
       }



       public bool CheckWeekend(DateTime date) 
       {
           DayOfWeek dayOfMovie = date.DayOfWeek;
           if ((dayOfMovie == DayOfWeek.Saturday) || (dayOfMovie == DayOfWeek.Sunday))
           {
               return true;
           }
           else return false;
       }

       public string GetBlockBusterStatus(string filmCode, int cinemaID) 
       {
           string status = string.Empty;

           if (cinemaID == 1) 
           {

               var rtn = getBlockBusterPalms(filmCode);
               status = rtn.IsBlockbuster;
           }
           if (cinemaID == 2)
           {

               var rtn = getBlockBusterPH(filmCode);
               status = rtn.IsBlockbuster;
           }
           if (cinemaID == 3)
           {

               var rtn = getBlockBusterMaryLand(filmCode);
               status = rtn.IsBlockbuster;
           }

           if (cinemaID == 4)
           {

               var rtn = getBlockBusterAbuja(filmCode);
               status = rtn.IsBlockbuster;
           }

           if (cinemaID == 5)
           {

               var rtn = getBlockBusterWarri(filmCode);
               status = rtn.IsBlockbuster;
           }

           if (cinemaID == 6)
           {

               var rtn = getBlockBusterOwerri(filmCode);
               status = rtn.IsBlockbuster;
           }

           if (cinemaID == 7)
           {

               var rtn = getBlockBusterAjah(filmCode);
               status = rtn.IsBlockbuster;
           }

           if (cinemaID == 8)
           {

               var rtn = getBlockBusterAsaba(filmCode);
               status = rtn.IsBlockbuster;
           }

           if (cinemaID == 9)
           {

               var rtn = getBlockBusterGateway(filmCode);
               status = rtn.IsBlockbuster;
           }


           return status;
       
       
       }
       public BlockBluster getBlockBusterPalms(string filmCode)
       {
           try
           {

               DynamicParameters param = new DynamicParameters();
               param.Add("@filmCode", filmCode);

               var result = db.Query<BlockBluster>(sql: "Isp_GetIsBlockBusterPalms",
                   param: param, commandType: CommandType.StoredProcedure);


               return result.FirstOrDefault();
           }
           catch (Exception)
           {


           }
           return null;


       }

       public BlockBluster getBlockBusterPH(string filmCode)
       {
           try
           {

               DynamicParameters param = new DynamicParameters();
               param.Add("@filmCode", filmCode);

               var result = db.Query<BlockBluster>(sql: "Isp_GetIsBlockBusterPH",
                   param: param, commandType: CommandType.StoredProcedure);


               return result.FirstOrDefault();
           }
           catch (Exception)
           {


           }
           return null;

       }
       public BlockBluster getBlockBusterMaryLand(string filmCode)
       {
           try
           {

               DynamicParameters param = new DynamicParameters();
               param.Add("@filmCode", filmCode);

               var result = db.Query<BlockBluster>(sql: "Isp_GetIsBlockBusterMaryLand",
                   param: param, commandType: CommandType.StoredProcedure);


               return result.FirstOrDefault();
           }
           catch (Exception)
           {


           }
           return null;

       }


       public BlockBluster getBlockBusterAbuja(string filmCode)
       {
           try
           {

               DynamicParameters param = new DynamicParameters();
               param.Add("@filmCode", filmCode);

               var result = db.Query<BlockBluster>(sql: "Isp_GetIsBlockBusterAbuja",
                   param: param, commandType: CommandType.StoredProcedure);


               return result.FirstOrDefault();
           }
           catch (Exception)
           {


           }
           return null;

       }

       public BlockBluster getBlockBusterWarri(string filmCode)
       {
           try
           {

               DynamicParameters param = new DynamicParameters();
               param.Add("@filmCode", filmCode);

               var result = db.Query<BlockBluster>(sql: "Isp_GetIsBlockBusterWarri",
                   param: param, commandType: CommandType.StoredProcedure);


               return result.FirstOrDefault();
           }
           catch (Exception)
           {


           }
           return null;

       }

       public BlockBluster getBlockBusterOwerri(string filmCode)
       {
           try
           {

               DynamicParameters param = new DynamicParameters();
               param.Add("@filmCode", filmCode);

               var result = db.Query<BlockBluster>(sql: "Isp_GetIsBlockBusterOwerri",
                   param: param, commandType: CommandType.StoredProcedure);


               return result.FirstOrDefault();
           }
           catch (Exception)
           {


           }
           return null;

       }

       public BlockBluster getBlockBusterAjah(string filmCode)
       {
           try
           {

               DynamicParameters param = new DynamicParameters();
               param.Add("@filmCode", filmCode);

               var result = db.Query<BlockBluster>(sql: "Isp_GetIsBlockBusterAjah",
                   param: param, commandType: CommandType.StoredProcedure);


               return result.FirstOrDefault();
           }
           catch (Exception)
           {


           }
           return null;

       }
       public BlockBluster getBlockBusterAsaba(string filmCode)
       {
           try
           {

               DynamicParameters param = new DynamicParameters();
               param.Add("@filmCode", filmCode);

               var result = db.Query<BlockBluster>(sql: "Isp_GetIsBlockBusterAsaba",
                   param: param, commandType: CommandType.StoredProcedure);


               return result.FirstOrDefault();
           }
           catch (Exception)
           {


           }
           return null;

       }
       public BlockBluster getBlockBusterGateway(string filmCode)
       {
           try
           {

               DynamicParameters param = new DynamicParameters();
               param.Add("@filmCode", filmCode);

               var result = db.Query<BlockBluster>(sql: "Isp_GetIsBlockBusterGateway",
                   param: param, commandType: CommandType.StoredProcedure);


               return result.FirstOrDefault();
           }
           catch (Exception)
           {


           }
           return null;

       }


       public class BlockBluster
       {
           public string IsBlockbuster { get; set; }
       

       }
    }
}
