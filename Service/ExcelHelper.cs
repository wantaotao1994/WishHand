using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WishHand.Service
{
    public class ExcelHelper
    {
        /// <summary>
        ///   读取excel
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        public IList<List<string>> ReadExcel(string  filePath,int  head=1)
        {
            FileInfo fileInfo = new FileInfo(filePath);

            IList<List<string>> list = new List<List<string>>();
           if (!fileInfo.Exists)
            {
                return list;
            }
            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];  //目前只支持第一个工作薄
                int rowCount = worksheet.Dimension.Rows+1;
                int ColCount = worksheet.Dimension.Columns+1;


                for (int j = 1+head; j < rowCount+1; j++)  //调过第一行表头
                {
                    List <string> row = new List<string>();
                    for (int i = 1; i < ColCount; i++)  //从第一列
                    {
                        var sku = worksheet.Cells[j, i].Value;
                        if (sku == null)
                        {
                            continue;
                        }
                        
                        row.Add(sku.ToString());
                    }

                    list.Add(row);
                }

                return list;
            }
        }


        public void CreateExcel(IList<IList<string>> list, string fileName)
        {
            FileInfo file = new FileInfo(fileName);
    
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(fileName);
            }
            using (ExcelPackage package = new ExcelPackage(file))
            {
                //创建sheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("sheet1");

                worksheet.InsertColumn(1,list.Count);
                worksheet.InsertRow( 1,list[0].Count);



                for (int i = 0; i < list.Count; i++)
                {
                    for (int j = 0; j < list[i].Count; j++)
                    {
                        worksheet.Cells[j+1, i+1].Value= list[i][j];
                    }
                }
                package.Save(); //Save the workbook.
               
            }
        }
    }
}
