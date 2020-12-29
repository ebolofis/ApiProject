using Dapper;
using Excel;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Symposium_AddProductsFromFile
{
    public partial class fMain : Form
    {
        /// <summary>
        /// Datase With All data from Excel
        /// </summary>
        DataSet dsExcel = new DataSet();

        /// <summary>
        /// Selected Sheet with Data
        /// </summary>
        DataTable dtExcel = new DataTable();

        /// <summary>
        /// List of rate codes
        /// </summary>
        List<RateCodes> rates = new List<RateCodes>();

        /// <summary>
        /// List of units from DB
        /// </summary>
        List<Units> units = new List<fMain.Units>();

        /// <summary>
        /// Connection string from app.config
        /// </summary>
        string Connection = "";

        public fMain()
        {
            InitializeComponent();
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSelFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "Excel Workbook 97-2003 (xls)|*.xls|Excel Workbook (xlsx)|*.xlsx";
            if (file.ShowDialog() == DialogResult.OK)
            {
                ctlSelFile.Text = file.FileName;
            }
        }

        private void btnReadWorkBook_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                if (ctlSelSheet.SelectedIndex > -1)
                {
                    dtExcel = dsExcel.Tables[ctlSelSheet.SelectedIndex];
                    lblTotRecs.Text = "Total records : " + dtExcel.Rows.Count.ToString();
                    grdMain.DataSource = dtExcel;
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void mnuLoad_Click(object sender, EventArgs e)
        {
            if (!File.Exists(ctlSelFile.Text))
            {
                MessageBox.Show("File " + ctlSelFile.Text + " not exists", "File not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                FileStream fs = File.Open(ctlSelFile.Text, FileMode.Open, FileAccess.Read);
                IExcelDataReader reader;
                if (ctlSelFile.Text.EndsWith(".xls"))
                    reader = ExcelReaderFactory.CreateBinaryReader(fs);
                else
                    reader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                reader.IsFirstRowAsColumnNames = chkFirstRow.Checked;
                dsExcel = reader.AsDataSet();
                ctlSelSheet.Items.Clear();
                foreach (DataTable dt in dsExcel.Tables)
                {
                    ctlSelSheet.Items.Add(dt.TableName);
                }
                ctlSelSheet.SelectedIndex = 0;
                reader.Close();
            }
            catch(Exception ex)
            {
                string sMess = "";
                if (ex.Message.Contains("because it is being used by another process"))
                    sMess = "File is open. Please close file and try again";
                else
                    sMess = "Can not read file";
                MessageBox.Show(sMess, "Read file", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
        }

        private void mnuReadData_Click(object sender, EventArgs e)
        {
            btnReadWorkBook_Click(btnReadWorkBook, e);
        }

        private void fMain_Load(object sender, EventArgs e)
        {
            lblTotRecs.Text = "";
            lblProg.Text = "";
            Connection = ConfigurationManager.AppSettings["ConnectionString"];
            try
            {
                Cursor = Cursors.WaitCursor;
                using (SqlConnection cn = new SqlConnection(Connection))
                {
                    cn.Open();
                    string sql = "SELECT DISTINCT p.Id, p.[Description] FROM Pricelist p";
                    rates = cn.Query<RateCodes>(sql).ToList();

                    sql = "SELECT DISTINCT p.Id, p.[Description] FROM Units p";
                    units = cn.Query<Units>(sql).ToList();
                }
                ctlRateCode.Items.Clear();
                ctlCostRateCode.Items.Clear();
                foreach (RateCodes item in rates)
                {
                    ctlRateCode.Items.Add(item);
                    ctlCostRateCode.Items.Add(item);
                }
                if (rates.Count > 0)
                    ctlRateCode.SelectedIndex = 0;

                ctlUnit.Items.Clear();
                foreach (Units item in units)
                {
                    ctlUnit.Items.Add(item);
                }
                if (units.Count > 0)
                    ctlUnit.SelectedIndex = 0;
            }
            catch(Exception ex)
            {
                ctlErrors.Text += "Connection Error : \r\n" + ex.ToString();
                mnuStart.Enabled = false;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void mnuStart_Click(object sender, EventArgs e)
        {
            StringBuilder SQL = new StringBuilder();
            try
            {
                Cursor = Cursors.WaitCursor;

                object a = ctlRateCode.SelectedItem;
                long RatecodeId = ((RateCodes)a).Id;

                long CostRateCode = -1;
                if(ctlCostRateCode.SelectedIndex > -1)
                {
                    a = ctlCostRateCode.SelectedItem;
                    CostRateCode = ((RateCodes)a).Id;
                }

                a = ctlUnit.SelectedItem;
                long UnitId = ((Units)a).Id;

                int rejected = 0;
                int totRec = dtExcel.Rows.Count;

                string[] sBarCodes;

                List<StringProduct> Products;

                prgMain.Maximum = totRec;
                prgMain.Value = 0;
                Application.DoEvents();
                using (SqlConnection cn = new SqlConnection(Connection))
                {
                    cn.Open();
                    foreach (DataRow row in dtExcel.Rows)
                    {
                        StringProduct pr = new StringProduct();
                        Products = new List<StringProduct>();

                        if (row.ItemArray.Count() > 7)
                        {
                            if (row[6].ToString().IndexOf(',') > 0)
                            {
                                sBarCodes = row[6].ToString().Split(',');
                                foreach (string item in sBarCodes)
                                {
                                    if (!string.IsNullOrEmpty(item))
                                    {
                                        prgMain.Maximum++;
                                        pr = new StringProduct
                                        {
                                            Code = row[0].ToString(),
                                            Description = row[1].ToString(),
                                            Price = row[2].ToString(),
                                            PrCategory = row[3].ToString(),
                                            Vat = row[4].ToString(),
                                            Category = row[5].ToString(),
                                            BarCode = item,
                                            PrepTime = row[7].ToString(),
                                            CostPrice = row[8].ToString()
                                        };
                                        Products.Add(pr);
                                    }
                                }
                            }
                            else
                            {
                                pr = new StringProduct
                                {
                                    Code = row[0].ToString(),
                                    Description = row[1].ToString(),
                                    Price = row[2].ToString(),
                                    PrCategory = row[3].ToString(),
                                    Vat = row[4].ToString(),
                                    Category = row[5].ToString(),
                                    BarCode = row[6].ToString(),
                                    PrepTime = row[7].ToString(),
                                    CostPrice = row[8].ToString()
                                };
                                Products.Add(pr);
                            }
                        }
                        else
                        {
                            if (row[6].ToString().IndexOf(',') > 0)
                            {
                                sBarCodes = row[6].ToString().Split(',');
                                foreach (string item in sBarCodes)
                                {
                                    if (!string.IsNullOrEmpty(item))
                                    {
                                        prgMain.Maximum++;
                                        pr = new StringProduct
                                        {
                                            Code = row[0].ToString(),
                                            Description = row[1].ToString(),
                                            Price = row[2].ToString(),
                                            PrCategory = row[3].ToString(),
                                            Vat = row[4].ToString(),
                                            Category = row[5].ToString(),
                                            BarCode = item,
                                            PrepTime = row[7].ToString()
                                        };
                                        Products.Add(pr);
                                    }
                                }                                
                            }
                            else
                            {
                                pr = new StringProduct
                                {
                                    Code = row[0].ToString(),
                                    Description = row[1].ToString(),
                                    Price = row[2].ToString(),
                                    PrCategory = row[3].ToString(),
                                    Vat = row[4].ToString(),
                                    Category = row[5].ToString(),
                                    BarCode = row[6].ToString(),
                                    PrepTime = row[7].ToString()
                                };
                                Products.Add(pr);
                            }
                        }

                        foreach (StringProduct item in Products)
                        {
                            if (item.Vat.StartsWith("0,") || item.Vat.StartsWith("0."))
                            {
                                item.Vat = double.Parse(item.Vat.Replace(",", ".")).ToString();
                            }
                            lblProg.Text = "Record : " + prgMain.Value + " / " + totRec + " Current product code " + item.Code + " for " + item.Description;
                            prgMain.Value++;
                            Application.DoEvents();
                            if (!AddValuesToWebPos(RatecodeId, CostRateCode, UnitId, item, cn))
                            {
                                ctlErrors.Text += "Product with code : " + item.Code + "  and description : " + item.Description + " not added \r\n";
                                rejected++;
                            }
                        }
                    }
                }
                lblProg.Text = "Inserted products " + (totRec - rejected).ToString() + ", Rejected products " + rejected.ToString();
            }
            catch (Exception ex)
            {
                ctlErrors.Text += "mnuStart : [SQL : " + SQL.ToString() + "] \r\n" + ex.ToString();
            }
            finally
            {
                prgMain.Value = 0;
                Cursor = Cursors.Default;
                Application.DoEvents();
            }
        }

        /// <summary>
        /// Add product to WebPos
        /// </summary>
        /// <param name="Product"></param>
        /// <param name="cn"></param>
        /// <returns></returns>
        private bool AddValuesToWebPos(long RateCodeId, long CostRateCodeId, long UnitId, StringProduct Product, SqlConnection cn)
        {
            bool res = true;
            StringBuilder SQL = new StringBuilder();
            try
            {
                long CategoryId = 0;
                long ProductCategoriesId = 0;
                long VatId = -1;
                long ProductId = -1;
                int tmpRes = 0;

                Product.BarCode = RemoveSpecialCharacters(Product.BarCode, 250);
                Product.Category = RemoveSpecialCharacters(Product.Category, 500);
                Product.PrCategory = RemoveSpecialCharacters(Product.PrCategory, 150);
                Product.Description = RemoveSpecialCharacters(Product.Description, 50);
                Product.Code = RemoveSpecialCharacters(Product.Code, 150);

                SqlCommand cm = new SqlCommand();
                SQL.Clear();
                SQL.Append("IF EXISTS(SELECT 1 FROM Product AS p WHERE p.Code = '" + Product.Code + "') \n"
                           + "	SELECT Id FROM Product AS p WHERE p.Code = '" + Product.Code + "' \n"
                           + "ELSE \n"
                           + "	SELECT 0 res");
                tmpRes = cn.Query<int>(SQL.ToString()).FirstOrDefault();
                if(tmpRes != 0)
                {
                    if (!string.IsNullOrEmpty(Product.BarCode))
                    {
                        SQL.Clear();
                        SQL.Append("IF NOT EXISTS (SELECT 1 FROM ProductBarcodes AS pb WHERE pb.Barcode = '" + Product.BarCode + "' AND pb.ProductId = " + tmpRes.ToString() + ") \n"
                                   + "	INSERT INTO ProductBarcodes (Barcode, ProductId, [Type]) \n"
                                   + "	SELECT '" + Product.BarCode + "'," + tmpRes.ToString() + ",NULL");
                        cm = new SqlCommand(SQL.ToString(), cn);
                        cm.ExecuteNonQuery();
                    }
                    ctlErrors.Text += "Product code " + Product.Code + " exists \r\n";
                    return false;
                }

                //Get's Vat Id
                SQL.Clear();
                SQL.Append("IF EXISTS(SELECT 1 FROM Vat AS v WHERE v.Percentage = " + Product.Vat.Replace(",", ".") + ") \n"
                           + "	SELECT Id FROM Vat AS v WHERE v.Percentage = " + Product.Vat.Replace(",", ".") + " \n"
                           + "ELSE \n"
                           + "	SELECT -1");
                VatId = cn.Query<long>(SQL.ToString()).FirstOrDefault();
                if(VatId < 1)
                {
                    ctlErrors.Text += "Vat Percentage " + Product.Vat.Replace(",",".") + " not exists \r\n";
                    return false;
                }

                //Get's category id
                SQL.Clear();
                SQL.Append("IF NOT EXISTS (SELECT 1 FROM Categories AS c WHERE c.Description = '" + Product.Category + "') \n"
                           + "	INSERT INTO Categories (Description,Status) VALUES ('" + Product.Category + "',1) \n"
                           + "SELECT Id FROM Categories WHERE Description = '" + Product.Category + "'");
                CategoryId = cn.Query<long>(SQL.ToString()).FirstOrDefault();

                //Get's ProductCategory id
                SQL.Clear();
                SQL.Append("IF NOT EXISTS (SELECT 1 FROM ProductCategories AS pc WHERE pc.Description = '" + Product.PrCategory + "') \n"
                           + "	INSERT INTO ProductCategories ([Description], [Type], [Status], Code, CategoryId) \n"
                           + "	SELECT '" + Product.PrCategory + "',0,1, MAX(CAST(pc.Code AS BIGINT))+1," + CategoryId.ToString() + "   \n"
                           + "	FROM ProductCategories AS pc \n"
                           + "	WHERE ISNUMERIC(pc.Code) = 1 \n"
                           + "SELECT Id FROM ProductCategories WHERE [Description] = '" + Product.PrCategory + "'");
                ProductCategoriesId = cn.Query<long>(SQL.ToString()).FirstOrDefault();

                SQL.Clear();
                SQL.Append("INSERT INTO Product ([Description], ExtraDescription, SalesDescription, Qty, \n"
                       + "	UnitId, PreparationTime, KdsId, KitchenId, ImageUri, ProductCategoryId, \n"
                       + "	Code, IsCustom, KitchenRegionId, IsDeleted) \n"
                       + "SELECT '" + Product.Description + "','" + Product.Description + "','" + Product.Description + "',NULL, \n"
                       + "  " + UnitId.ToString() + ", " + Product.PrepTime + ",NULL,NULL,NULL," + ProductCategoriesId.ToString() + ", \n"
                       + "  '" + Product.Code + "', 0,NULL,NULL");
                cm = new SqlCommand(SQL.ToString(), cn);
                cm.ExecuteNonQuery();

                SQL.Clear();
                SQL.Append("SELECT Id FROM Product WHERE Code = '" + Product.Code + "'");
                ProductId = cn.Query<long>(SQL.ToString()).FirstOrDefault();

                SQL.Clear();
                SQL.Append("IF NOT EXISTS (SELECT 1 FROM PricelistDetail AS pd WHERE pd.PricelistId = " + RateCodeId.ToString() + " AND pd.ProductId = " + ProductId.ToString() + " AND pd.Price = " + Product.Price.Replace(",", ".") + " AND pd.VatId = " + VatId.ToString() + ") \n"
                           + "	INSERT INTO PricelistDetail(PricelistId, ProductId, IngredientId, Price, VatId, TaxId, PriceWithout, MinRequiredExtras) \n"
                           + "	SELECT " + RateCodeId.ToString() + "," + ProductId.ToString() + ",NULL," + Product.Price.Replace(",", ".") + "," + VatId.ToString() + ",NULL,0,NULL");
                cm = new SqlCommand(SQL.ToString(), cn);
                cm.ExecuteNonQuery();

                decimal chkDec;
                bool isNumeric = false;
                isNumeric = decimal.TryParse(Product.CostPrice.Replace(",", "."), out chkDec);
                if (CostRateCodeId>-1 && !string.IsNullOrEmpty(Product.CostPrice) && isNumeric)
                {
                    SQL.Clear();
                    SQL.Append("IF NOT EXISTS (SELECT 1 FROM PricelistDetail AS pd WHERE pd.PricelistId = " + CostRateCodeId.ToString() + " AND pd.ProductId = " + ProductId.ToString() + " AND pd.Price = " + Product.CostPrice.Replace(",", ".") + " AND pd.VatId = " + VatId.ToString() + ") \n"
                               + "	INSERT INTO PricelistDetail(PricelistId, ProductId, IngredientId, Price, VatId, TaxId, PriceWithout, MinRequiredExtras) \n"
                               + "	SELECT " + CostRateCodeId.ToString() + "," + ProductId.ToString() + ",NULL," + Product.CostPrice.Replace(",", ".") + "," + VatId.ToString() + ",NULL,0,NULL");
                    cm = new SqlCommand(SQL.ToString(), cn);
                    cm.ExecuteNonQuery();
                }

                

                if (!string.IsNullOrEmpty(Product.BarCode))
                {
                    SQL.Clear();
                    SQL.Append("IF NOT EXISTS (SELECT 1 FROM ProductBarcodes AS pb WHERE pb.Barcode = '" + Product.BarCode + "' AND pb.ProductId = " + ProductId.ToString() + ") \n"
                               + "	INSERT INTO ProductBarcodes (Barcode, ProductId, [Type]) \n"
                               + "	SELECT '" + Product.BarCode + "'," + ProductId.ToString() + ",NULL ");
                    cm = new SqlCommand(SQL.ToString(), cn);
                    cm.ExecuteNonQuery();
                }

            }
            catch(Exception ex)
            {
                ctlErrors.Text += "AddValuesToWebPos [SQL: " + SQL.ToString() + "] \r\n " + ex.ToString();
                res = false;
            }
            return res;
        }

        /// <summary>
        /// Removes special chars as ',",%....
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string RemoveSpecialCharacters(string str, int length)
        {
            StringBuilder sb = new StringBuilder();
            string sRec = "";
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'Α' && c <= 'Ω') || 
                    (c >= 'a' && c <= 'z') || (c >= 'α' && c <= 'ω') || c == '.' || c == '_' || c==' ' || c=='(' || c == ')') 
                {
                    sRec += c;
                    //sb.Append(c);
                }
            }
            if (sRec.Length > length)
                sRec = sRec.Substring(0, length);
            //return sb.ToString();
            return sRec;
        }

        /// <summary>
        /// Model for Rate codes
        /// </summary>
        public class RateCodes
        {
            /// <summary>
            /// Id 
            /// </summary>
            public long Id { get; set; }

            /// <summary>
            /// description
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Returns Description for combo boc
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return Description;
            }
        }

        /// <summary>
        /// Model for Units
        /// </summary>
        public class Units
        {
            /// <summary>
            /// Id 
            /// </summary>
            public long Id { get; set; }

            /// <summary>
            /// description
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Returns Description for combo boc
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return Description;
            }
        }

        /// <summary>
        /// Product from Excel to String fields
        /// </summary>
        public class StringProduct
        {
            /// <summary>
            /// Product Code
            /// </summary>
            public string Code { get; set; }

            /// <summary>
            /// Product Description
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Product Price
            /// </summary>
            public string Price { get; set; }

            /// <summary>
            /// Product Product Category
            /// </summary>
            public string PrCategory { get; set; }

            /// <summary>
            /// Product Price List Vat
            /// </summary>
            public string Vat { get; set; }

            /// <summary>
            /// Product Category
            /// </summary>
            public string Category { get; set; }

            /// <summary>
            /// Product Barcode
            /// </summary>
            public string BarCode { get; set; }

            /// <summary>
            /// Product Preparation Time
            /// </summary>
            public string PrepTime { get; set; }

            /// <summary>
            /// Cost Price
            /// </summary>
            public string CostPrice { get; set; }
        }
    }
}
