using log4net;
using Symposium.Helpers;
using Symposium.Helpers.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace PMSConnectionLib
{
	public class PMSConnection
	{
		private SqlConnection connection;
		private string connectString;
		public ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		Dictionary<string, dynamic> configuration = MainConfigurationHelper.GetMainConfiguration("api");

		/// <summary>
		/// Creates variable for connection
		/// </summary>
		/// <param name="connStr"></param>
		/// <returns></returns>
		public bool initConn(string connStr)
		{
			connectString = connStr;
			if (connection == null)
			{
				logger.Info("ConString is null.");
				try
				{
					connection = new SqlConnection(connStr);

					return true;
				}
				catch (Exception ex)
				{
					logger.Error(ex.ToString());
					var a = ex.Message;
					return false;
				}

			}
			else
			{
				if (connection.State != ConnectionState.Closed)
				{
					logger.Info("ConString is not Closed.");
					connection.Close();
				}
				connection.ConnectionString = connStr;
				connection.Open();
				logger.Info("ConString is Open.");
				return true;
			}
		}

		/// <summary>
		/// Checks Connection state (Close or Open)
		/// </summary>
		public void closeConnection()
		{
			connection.Close();
		}

		/// <summary>
		/// Checks if variable for connection is null or not
		/// </summary>
		/// <returns></returns>
		public bool checkConnection()
		{
			return connection == null;
		}

		/// <summary>
		/// Creare sp to pms's DB (Protel or Ermis)
		/// </summary>
		/// <param name="username">pms's DB username</param>
		/// <param name="hotelType">PROTEL or ERMIS</param>
		/// <returns></returns>
		public bool makeProcedure(string username, string hotelType)
		{
			bool execeted = true;
			bool res = false;
			if (configuration["MakeProcedure"] == true)
			{
				StringBuilder command = new StringBuilder();
				command.Clear();
				switch (hotelType)
				{
					case "PROTEL":
						res = checkIfLoyaltyTablesExists(username);
						if (execeted)
							execeted = res;

                        //New store proceudre GetReservationInfoPeriod to get data from protel for a period and not for hotel date
                        command.Clear();
                        command.Append(retReservationInfoPeriod(username));
                        if(command.Length > 10)
                        {
                            res = execSQL(command.ToString());
                            if (execeted)
                                execeted = res;
                        }

                        //New store proceudre GetReservationInfoPeriodAllReservs to get data from protel for a period and not for hotel date
                        command.Clear();
                        command.Append(retReservationInfoPeriodAllReserv(username));
                        if (command.Length > 10)
                        {
                            res = execSQL(command.ToString());
                            if (execeted)
                                execeted = res;
                        }

                        //Old procedure GetReservationInfo2 calls GetReservationInfoPeriod for hotel date as period
                        command.Clear();
						command.Append(retReservationInfo(username));
						if (command.Length > 10)
						{
							res = execSQL(command.ToString());
							if (execeted)
								execeted = res;
						}
						command.Clear();
						command.Append(retProtelDepartments(username));
						res = execSQL(command.ToString());
						if (execeted)
							execeted = res;
						command.Clear();
						command.Append(ProtelCustomersByAFM(username));
						res = execSQL(command.ToString());
						if (execeted)
							execeted = res;
						command.Clear();
						command.Append(ProtelMethodsOfPayment());
						res = execSQL(command.ToString());
						if (execeted)
							execeted = res;
						command.Clear();
						command.Append(ProtelInvoiceTypes());
						res = execSQL(command.ToString());
						if (execeted)
							execeted = res;
						break;
					case "ERMIS":
						command.Append(retErmisResInfo(username));
						if (command.Length > 10)
						{
							res = execSQL(command.ToString());
							if (execeted)
								execeted = res;
						}
						command.Clear();
						command.Append(retErmisDep(username));
						if (command.Length > 10)
						{
							res = execSQL(command.ToString());
							if (execeted)
								execeted = res;
						}
						command.Clear();
						command.Append(ErmisCustomersByAFM(username));
						if (command.Length > 10)
						{
							res = execSQL(command.ToString());
							if (execeted)
								execeted = res;
						}
						command.Clear();
						command.Append(ErmisMethodsOfPayment());
						if (command.Length > 10)
						{
							res = execSQL(command.ToString());
							if (execeted)
								execeted = res;
						}
						command.Clear();
						command.Append(ErmisInvoiceTypes());
						if (command.Length > 10)
						{
							res = execSQL(command.ToString());
							if (execeted)
								execeted = res;
						}
						break;
				}
			}
			else
			{
				logger.Warn("Store Procedure GetReservationInfo and GetReservationInfo2 does not Created due to Configuration banned!");
				execeted = false;
			}
			return execeted;
		}

		/// <summary>
		/// Execute SQL Statment (Insert, Delete, Update NOT SELECT)
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		public bool execSQL(string command)
		{
			try
			{
				if (connection.State == ConnectionState.Open)
				{
					connection.Close();
				}

				using (connection)
				{
					connection.ConnectionString = connectString;
					connection.Open();
					SqlCommand comm = new SqlCommand(command, connection);
					comm.ExecuteNonQuery();
				}

				return true;
			}
			catch (Exception ex1)
			{
				logger.Error(ex1.ToString());
				var b = ex1.Message;
				return false;
			}
		}

		/// <summary>
		/// Execute SQL Statment (Insert into Actions)
		/// </summary>
		/// <param name="command"></param>
		/// <param name="kdnr"></param>
		/// <param name="actiondate"></param>
		/// <param name="type"></param>
		/// <param name="points"></param>
		/// <param name="notes"></param>
		/// <returns></returns>
		public bool execSQLForLoyalty(string command, int? kdnr, DateTime? actiondate, int type, int points, string notes)
		{
			try
			{
				if (connection.State == ConnectionState.Open)
				{
					connection.Close();
				}

				using (connection)
				{
					connection.ConnectionString = connectString;
					connection.Open();
					SqlCommand comm = new SqlCommand(command, connection);
					comm.Parameters.AddWithValue("@kdnr", kdnr);
					comm.Parameters.AddWithValue("@actiondate", actiondate.Value.Date);
					comm.Parameters.AddWithValue("@type", type);
					comm.Parameters.AddWithValue("@points", points);
					comm.Parameters.AddWithValue("@notes", notes);
					comm.ExecuteNonQuery();
				}

				return true;
			}
			catch (Exception ex1)
			{
				logger.Error(ex1.ToString());
				var b = ex1.Message;
				return false;
			}
		}

		/// <summary>
		/// Execute SQL Statment (Insert into Actions)
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		public bool execSQLForDeleteCustomers(string command)
		{
			try
			{
				if (connection.State == ConnectionState.Open)
				{
					connection.Close();
				}

				using (connection)
				{
					connection.ConnectionString = connectString;
					connection.Open();
					SqlCommand comm = new SqlCommand(command, connection);
					comm.ExecuteNonQuery();
				}

				return true;
			}
			catch (Exception ex1)
			{
				logger.Error(ex1.ToString());
				var b = ex1.Message;
				return false;
			}
		}

		/// <summary>
		/// Returns Hotel Date from PMs
		/// </summary>
		/// <param name="username"></param>
		/// <param name="hotelType"></param>
		/// <param name="mpeHotel"></param>
		/// <returns></returns>
		public DateTime getFODay(string username, string hotelType, int mpeHotel)
		{
			string commd = string.Empty;

			switch (hotelType)
			{
				case "PROTEL":
					commd = "SELECT d.pdate FROM " + username + ".datum d WHERE d.mpehotel = " + mpeHotel.ToString();
					break;
				case "ERMIS":
					commd = "SELECT begendat pdate FROM " + username + ".begen";
					break;
			}
			SqlDataReader da = returnResult(commd);
			da.Read();
			DateTime res = da.GetDateTime(0);
			da.Close();
			closeConnection();
			return res;
		}

		/// <summary>
		/// Return result from store procedures
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		public SqlDataReader returnResult(string command)
		{
			try
			{
				if (connection.State == ConnectionState.Open)
				{
					connection.Close();
				}
				connection.ConnectionString = connectString;
				connection.Open();
				SqlCommand sqlComm = new SqlCommand(command, connection);
				return sqlComm.ExecuteReader();
			}
			catch (Exception msg)
			{
				logger.Error(msg.ToString());
				var g = msg.Message;
				throw new Exception(g);
			}
		}

		public IEnumerable<dynamic> DataTableToJSON(SqlDataReader dr, bool dyn)
		{
			var expandoObject = new ExpandoObject() as IDictionary<string, object>;
			while (dr.Read())
			{
				yield return SqlDataReaderToExpando(dr);
			}
		}

		private dynamic SqlDataReaderToExpando(SqlDataReader reader)
		{
			var expandoObject = new ExpandoObject() as IDictionary<string, object>;
			for (var i = 0; i < reader.FieldCount; i++)
				expandoObject.Add(reader.GetName(i), reader[i]);

			return expandoObject;
		}

		/// <summary>
		/// Checks if Tables for Loyalty exists on DB and creates them
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		private bool checkIfLoyaltyTablesExists(string user)
		{
			bool res = true;
			StringBuilder sComms = new StringBuilder();
			try
			{
				sComms.Clear();
				sComms.Append("IF NOT EXISTS (SELECT 1 FROM sys.tables AS t WHERE t.name = 'hit_loyalty_actions_type') \n"
							   + "	CREATE TABLE [" + user + "].[hit_loyalty_actions_type]( \n"
							   + "		[id] [int] NOT NULL, \n"
							   + "		[description] [nvarchar](50) NOT NULL, \n"
							   + "		CONSTRAINT [PK_hit_loyalty_actions_type] PRIMARY KEY CLUSTERED  \n"
							   + "		( \n"
							   + "			[id] ASC \n"
							   + "		) \n"
							   + "	) ON [PRIMARY]; \n"
							   + "IF NOT EXISTS (SELECT 1 FROM sys.tables AS t WHERE t.name = 'hit_loyalty_cards_status') \n"
							   + "	CREATE TABLE [" + user + "].[hit_loyalty_cards_status]( \n"
							   + "		[id] [int] NOT NULL, \n"
							   + "		[description] [nvarchar](50) NOT NULL, \n"
							   + "		CONSTRAINT [PK_hit_loyalty_cards_status] PRIMARY KEY CLUSTERED  \n"
							   + "		( \n"
							   + "			[id] ASC \n"
							   + "		) \n"
							   + "	) ON [PRIMARY]; \n"
							   + "IF NOT EXISTS (SELECT 1 FROM sys.tables AS t WHERE t.name = 'hit_loyalty_customer_type') \n"
							   + "	CREATE TABLE [" + user + "].[hit_loyalty_customer_type]( \n"
							   + "		[id] [int] NOT NULL, \n"
							   + "		[description] [nvarchar](90) NOT NULL, \n"
							   + "		CONSTRAINT [PK_hit_loyalty_customer_type] PRIMARY KEY CLUSTERED  \n"
							   + "		( \n"
							   + "			[id] ASC \n"
							   + "		) \n"
							   + "	) ON [PRIMARY]; \n"
							   + "IF NOT EXISTS (SELECT 1 FROM sys.tables AS t WHERE t.name = 'hit_loyalty_generalrules') \n"
							   + "	CREATE TABLE [" + user + "].[hit_loyalty_generalrules]( \n"
							   + "		[ratesale] [int] NOT NULL, \n"
							   + "		[ratebuy] [int] NOT NULL, \n"
							   + "		[initpoints] [int] NOT NULL, \n"
							   + "		[cardduration] [int] NOT NULL, \n"
							   + "		[pointsduration] [int] NOT NULL, \n"
							   + "		[cardprefix] [nvarchar](3) NULL \n"
							   + "	) ON [PRIMARY]; \n"
							   + "IF NOT EXISTS (SELECT 1 FROM sys.tables AS t WHERE t.name = 'hit_loyalty_revenue_type') \n"
							   + "	CREATE TABLE [" + user + "].[hit_loyalty_revenue_type]( \n"
							   + "		[id] [int] NOT NULL, \n"
							   + "		[description] [nvarchar](90) NOT NULL \n"
							   + "	) ON [PRIMARY]; \n"
							   + "IF NOT EXISTS (SELECT 1 FROM sys.tables AS t WHERE t.name = 'hit_loyalty_rules') \n"
							   + "	CREATE TABLE [" + user + "].[hit_loyalty_rules]( \n"
							   + "		[id] [int] IDENTITY(1,1) NOT NULL, \n"
							   + "		[customertype] [int] NOT NULL, \n"
							   + "		[revenuetype] [int] NOT NULL, \n"
							   + "		[roomtype] [int] NOT NULL, \n"
							   + "		[rewardrate] [int] NOT NULL, \n"
							   + "		[classType] [int] NULL, \n"
							   + "		CONSTRAINT [PK_hit_loyalty_rules] PRIMARY KEY CLUSTERED  \n"
							   + "		( \n"
							   + "			[id] ASC \n"
							   + "		) \n"
							   + "	) ON [PRIMARY]; \n"
							   + "IF NOT EXISTS (SELECT 1 FROM sys.tables AS t WHERE t.name = 'hit_loyalty_classes') \n"
							   + "	CREATE TABLE [" + user + "].[hit_loyalty_classes]( \n"
							   + "		[id] [int] IDENTITY(1,1) NOT NULL, \n"
							   + "		[name] [nvarchar](90) NOT NULL, \n"
							   + "		[threshold] [int] NOT NULL, \n"
							   + "		[image] [image] NULL, \n"
							   + "		[rgb] [nvarchar](10) NULL, \n"
							   + "		[fnbdiscount] [int] NULL, \n"
							   + "		CONSTRAINT [PK_hit_loyalty_classes] PRIMARY KEY CLUSTERED  \n"
							   + "		( \n"
							   + "			[id] ASC \n"
							   + "		) \n"
							   + "	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]; \n"
							   + "IF NOT EXISTS (SELECT 1 FROM sys.tables AS t WHERE t.name = 'hit_loyalty_kunden') \n"
							   + "BEGIN  \n"
							   + "	CREATE TABLE [" + user + "].[hit_loyalty_kunden]( \n"
							   + "		[kdnr] [int] NOT NULL, \n"
							   + "		[points] [int] NOT NULL, \n"
							   + "		[reward] [int] NOT NULL, \n"
							   + "		[password] [nvarchar](150) NULL, \n"
							   + "		[class] [int] NULL, \n"
							   + "		[cardcount] [int] NOT NULL, \n"
							   + "		[classChanged] [bit] NULL, \n"
							   + "		[ReceiveMails] [bit] NULL DEFAULT(1), \n"
							   + "		CONSTRAINT [PK_hit_loyalty_kunden_1] PRIMARY KEY CLUSTERED  \n"
							   + "		( \n"
							   + "			[kdnr] ASC \n"
							   + "		) \n"
							   + "	) ON [PRIMARY]; \n"
							   + " \n"
							   + "	ALTER TABLE [" + user + "].[hit_loyalty_kunden]  WITH CHECK ADD  CONSTRAINT [FK_hit_loyalty_kunden_hit_loyalty_classes] FOREIGN KEY([class]) \n"
							   + "	REFERENCES [" + user + "].[hit_loyalty_classes] ([id]); \n"
							   + " \n"
							   + "	ALTER TABLE [" + user + "].[hit_loyalty_kunden] CHECK CONSTRAINT [FK_hit_loyalty_kunden_hit_loyalty_classes]; \n"
							   + "END; \n"
							   + "IF NOT EXISTS (SELECT 1 FROM sys.tables AS t WHERE t.name = 'hit_loyalty_emailtemplates') \n"
							   + "	CREATE TABLE [" + user + "].[hit_loyalty_emailtemplates]( \n"
							   + "		[id] [int] NOT NULL, \n"
							   + "		[description] [nvarchar](250) NOT NULL, \n"
							   + "		[emailbody] [text] NOT NULL, \n"
							   + "		[emailsubject] [nvarchar](500) NOT NULL, \n"
							   + "		[IsActive] [bit] NULL DEFAULT(0), \n"
							   + "		CONSTRAINT [PK_hit_loyalty_emailtypes] PRIMARY KEY CLUSTERED  \n"
							   + "		( \n"
							   + "			[id] ASC \n"
							   + "		) \n"
							   + "	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]; \n"
							   + "IF NOT EXISTS (SELECT 1 FROM sys.tables AS t WHERE t.name = 'hit_loyalty_smtp') \n"
							   + "	CREATE TABLE [" + user + "].[hit_loyalty_smtp]( \n"
							   + "		[Id] [bigint] IDENTITY(1,1) NOT NULL, \n"
							   + "		[Smtp] [nvarchar](100) NOT NULL, \n"
							   + "		[Port] [int] NOT NULL, \n"
							   + "		[Ssl] [bit] NOT NULL, \n"
							   + "		[Username] [nvarchar](100) NOT NULL, \n"
							   + "		[Sender] [nvarchar](100) NOT NULL, \n"
							   + "		[Password] [nvarchar](500) NOT NULL, \n"
							   + "		[isActive] [bit] NOT NULL, \n"
							   + "		CONSTRAINT [PK_HESEmailConfig] PRIMARY KEY CLUSTERED  \n"
							   + "		( \n"
							   + "			[Id] ASC \n"
							   + "		) \n"
							   + "	) ON [PRIMARY]; \n"
							   + "IF NOT EXISTS (SELECT 1 FROM sys.tables AS t WHERE t.name = 'hit_loyalty_emailsendstack') \n"
							   + "	CREATE TABLE [" + user + "].[hit_loyalty_emailsendstack]( \n"
							   + "		[id] [int] IDENTITY(1,1) NOT NULL, \n"
							   + "		[kdnr] [int] NOT NULL, \n"
							   + "		[emailtemplate] [int] NOT NULL, \n"
							   + "		[status] [nvarchar](1000) NOT NULL, \n"
							   + "		[createDate] [datetime] NOT NULL, \n"
							   + "		CONSTRAINT [PK_hit_loyalty_emailsendstack] PRIMARY KEY CLUSTERED  \n"
							   + "		( \n"
							   + "			[id] ASC \n"
							   + "		) \n"
							   + "	) ON [PRIMARY]; \n"
							   + "IF NOT EXISTS (SELECT 1 FROM sys.tables AS t WHERE t.name = 'hit_loyalty_config') \n"
							   + "	CREATE TABLE [" + user + "].[hit_loyalty_config]( \n"
							   + "		[publicimgfolder] [nvarchar](500) NULL, \n"
							   + "		[privateimgfolder] [nvarchar](500) NULL \n"
							   + "	) ON [PRIMARY]; \n"
							   + "IF NOT EXISTS (SELECT 1 FROM sys.tables AS t WHERE t.name = 'hit_loyalty_actions') \n"
							   + "BEGIN  \n"
							   + "	CREATE TABLE [" + user + "].[hit_loyalty_actions]( \n"
							   + "		[id] [int] IDENTITY(1,1) NOT NULL, \n"
							   + "		[kdnr] [int] NOT NULL, \n"
							   + "		[actiondate] [datetime] NOT NULL, \n"
							   + "		[type] [int] NOT NULL, \n"
							   + "		[points] [int] NOT NULL, \n"
							   + "		[notes] [nvarchar](200) NULL, \n"
							   + "		CONSTRAINT [PK_hit_loyalty_actions_1] PRIMARY KEY CLUSTERED  \n"
							   + "		( \n"
							   + "			[id] ASC \n"
							   + "		) \n"
							   + "	) ON [PRIMARY]; \n"
							   + " \n"
							   + "	ALTER TABLE [" + user + "].[hit_loyalty_actions]  WITH CHECK ADD  CONSTRAINT [FK_hit_loyalty_actions_hit_loyalty_kunden] FOREIGN KEY([kdnr]) \n"
							   + "	REFERENCES [" + user + "].[hit_loyalty_kunden] ([kdnr]); \n"
							   + " \n"
							   + "	ALTER TABLE [" + user + "].[hit_loyalty_actions] CHECK CONSTRAINT [FK_hit_loyalty_actions_hit_loyalty_kunden]; \n"
							   + "END; \n"
							   + "IF NOT EXISTS (SELECT 1 FROM sys.tables AS t WHERE t.name = 'hit_loyalty_cards') \n"
							   + "BEGIN  \n"
							   + "	CREATE TABLE [" + user + "].[hit_loyalty_cards]( \n"
							   + "		[id] [int] IDENTITY(1,1) NOT NULL, \n"
							   + "		[kdnr] [int] NOT NULL, \n"
							   + "		[cardnum] [nvarchar](50) NOT NULL, \n"
							   + "		[issuedate] [datetime] NOT NULL, \n"
							   + "		[expdate] [datetime] NOT NULL, \n"
							   + "		[class] [int] NULL, \n"
							   + "		[status] [int] NOT NULL, \n"
							   + "		CONSTRAINT [PK_hit_loyalty_cards] PRIMARY KEY CLUSTERED  \n"
							   + "		( \n"
							   + "			[id] ASC \n"
							   + "		) \n"
							   + "	) ON [PRIMARY]; \n"
							   + " \n"
							   + "	ALTER TABLE [" + user + "].[hit_loyalty_cards]  WITH CHECK ADD  CONSTRAINT [FK_hit_loyalty_cards_hit_loyalty_classes] FOREIGN KEY([class]) \n"
							   + "	REFERENCES [" + user + "].[hit_loyalty_classes] ([id]); \n"
							   + " \n"
							   + "	ALTER TABLE [" + user + "].[hit_loyalty_cards] CHECK CONSTRAINT [FK_hit_loyalty_cards_hit_loyalty_classes]; \n"
							   + " \n"
							   + "	ALTER TABLE [" + user + "].[hit_loyalty_cards]  WITH CHECK ADD  CONSTRAINT [FK_hit_loyalty_cards_hit_loyalty_kunden] FOREIGN KEY([kdnr]) \n"
							   + "	REFERENCES [" + user + "].[hit_loyalty_kunden] ([kdnr]); \n"
							   + " \n"
							   + "	ALTER TABLE [" + user + "].[hit_loyalty_cards] CHECK CONSTRAINT [FK_hit_loyalty_cards_hit_loyalty_kunden]; \n"
							   + "END; \n"
							   + "IF NOT EXISTS (SELECT 1 FROM sys.tables AS t WHERE t.name = 'Hit_Loyalty_Kunden_Reserv') \n"
							   + "	CREATE TABLE [" + user + "].[Hit_Loyalty_Kunden_Reserv]( \n"
							   + "		[ID] [int] IDENTITY(1,1) NOT NULL, \n"
							   + "		[kdnr] [int] NOT NULL, \n"
							   + "		[buchnr] [int] NOT NULL, \n"
							   + "		[ptyp] [int] NOT NULL, \n"
							   + "		[points] [int] NOT NULL, \n"
							   + "		CONSTRAINT [PK_Hit_Loyalty_Kunden_Reserv] PRIMARY KEY CLUSTERED  \n"
							   + "		( \n"
							   + "			[ID] ASC \n"
							   + "		) \n"
							   + "	) ON [PRIMARY]; \n"
							   + "IF NOT EXISTS (SELECT 1 FROM sys.tables AS t WHERE t.name = 'hit_loyalty_classes_benefits') \n"
							   + "	CREATE TABLE [" + user + "].[hit_loyalty_classes_benefits]( \n"
							   + "		[id] [int] IDENTITY(1,1) NOT NULL, \n"
							   + "		[classid] [int] NOT NULL, \n"
							   + "		[position] [int] NOT NULL, \n"
							   + "		[description] [nvarchar](500) NOT NULL, \n"
							   + "		[isactive] [bit] NOT NULL, \n"
							   + "		CONSTRAINT [PK_hit_loyalty_classes_benefits] PRIMARY KEY CLUSTERED  \n"
							   + "		( \n"
							   + "			[id] ASC \n"
							   + "		) \n"
							   + "	) ON [PRIMARY];");
				res = execSQL(sComms.ToString());
			}
			catch (Exception ex)
			{
				logger.Error("checkIfLoyaltyTablesExists : " + ex.ToString());
				res = false;
			}
			return res;
		}

        /// <summary>
		/// create sp 'retReservationInfoPeriod' to protel DB to get reservation info, hotel's customers
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		private string retReservationInfoPeriod(string user)
        {
            //bool procExists = false;
            StringBuilder SQLCreate = new StringBuilder();
            string sCommand = "CREATE ";
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
            //bool b2000 = false;
            using (connection)
            {
                connection.ConnectionString = connectString;
                connection.Open();

                SqlDataAdapter da = new SqlDataAdapter("IF NOT EXISTS(SELECT 1 FROM sysobjects WHERE type = 'P' and name = 'GetReservationInfoPeriod') SELECT 0 nCnt ELSE SELECT 1 nCnt", connection);
                DataSet ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    if ((int)ds.Tables[0].Rows[0]["nCnt"] == 1)
                        sCommand = "ALTER ";
            }
            SQLCreate.Clear();
            SQLCreate.Append(sCommand + " PROCEDURE [" + user + "].[GetReservationInfoPeriod]  @confirmationCode VARCHAR(50), @dtFrom DATETIME, @dtTo DATETIME,  \n"
                               + "	@room VARCHAR(10), @mpeHotel INT, @pageNo INT, @pageSize INT, @bAllHotels BIT AS  \n"
                               + "BEGIN   \n"
                               + "	DECLARE @start INT, @ends INT, @idx INT, @tRec INT, @jdx INT, @jTRec INT  \n"
                               + "	SET @start = CASE WHEN @pageNo < 0 THEN 1 ELSE (@pageSize * @pageNo) + 1 END    \n"
                               + "	SET @ends = CASE WHEN @pageNo < 0 THEN 100000000 ELSE @pageSize * (@pageNo + 1) END   \n"
                               + "  \n"
                               + "	DECLARE @Resevs TABLE (ResId INT, ptyp INT)  \n"
                               + "	DECLARE @exRes TABLE (leistacc INT, kd INT)   \n"
                               + "	DECLARE @Profiles TABLE (leistacc INT, ProfileNo INT,FirstName VARCHAR(50),LastName VARCHAR(80),Member VARCHAR(50),  \n"
                               + "		Password VARCHAR(15),Address VARCHAR(80),City VARCHAR(50),PostalCode VARCHAR(17),Country VARCHAR(80),  \n"
                               + "		birthdayDT DATETIME,Email VARCHAR(75),Telephone VARCHAR(50),VIP INT,Benefits VARCHAR(250),  \n"
                               + "		NationalityCode VARCHAR(80), NatId INT, LoyaltyProgramId INT, LoyaltyLevelId INT, CardType VARCHAR(200))  \n"
                               + "	DECLARE @Loyalty TABLE (Kdnr INT, ClassId INT, ClassName NVARCHAR(90), fnbdiscount INT, AvailablePoints INT, ratebuy INT)  \n"
                               + "	  \n"
                               + "	DECLARE @Boards TABLE (leistacc INT, BoardCode VARCHAR(MAX) , BoardName VARCHAR(MAX), UNIQUE NONCLUSTERED(leistacc))  \n"
                               + "	DECLARE @Fetures TABLE (ProfileId INT, Short VARCHAR(MAX))  \n"
                               + "	  \n"
                               + "	IF ISNULL(@room,'') = ''  \n"
                               + "		INSERT INTO @Resevs(ResId, ptyp)  \n"
                               + "		SELECT DISTINCT b.leistacc, b.preistyp  \n"
                               + "		FROM buch AS b  \n"
                               + "		INNER JOIN kat AS k ON k.katnr = b.katnr AND k.grpkto <> 1  \n"
                               + "		INNER JOIN zimmer AS z ON z.zinr = b.zimmernr    \n"
                               + "		WHERE b.reschar < 2 AND (b.mpehotel = @mpeHotel OR @bAllHotels = 1) AND b.globdbis >= @dtFrom AND b.globdvon <= @dtTo AND  \n"
                               + "			((b.globbnr < 1) OR (b.globbnr > 0 AND b.umzdurch  =1)) AND   \n"
                               + "			CASE WHEN ISNULL(@confirmationCode,'') = '' THEN 1  \n"
                               + "				 WHEN ISNUMERIC(@confirmationCode) = 1 AND CAST(@confirmationCode AS INT) = b.leistacc THEN 1 \n"
                               + "				 WHEN CHARINDEX(@confirmationCode, b.string1) > 0 THEN 1  \n"
                               + "			ELSE 0 END = 1 AND b.buchstatus <> 0    \n"
                               + "	ELSE IF EXISTS(SELECT 1 FROM zimmer AS z WHERE CHARINDEX(@room, z.ziname) > 0)  \n"
                               + "		INSERT INTO @Resevs(ResId, ptyp)  \n"
                               + "		SELECT DISTINCT b.leistacc, b.preistyp  \n"
                               + "		FROM buch AS b  \n"
                               + "		INNER JOIN kat AS k ON k.katnr = b.katnr AND k.grpkto <> 1  \n"
                               + "		INNER JOIN zimmer AS z ON z.zinr = b.zimmernr AND z.ziname = @room   \n"
                               + "		WHERE b.reschar < 2 AND (b.mpehotel = @mpeHotel OR @bAllHotels = 1) AND b.globdbis >= @dtFrom AND b.globdvon <= @dtTo AND  \n"
                               + "			((b.globbnr < 1) OR (b.globbnr > 0 AND b.umzdurch  =1)) AND   \n"
                               + "			CASE WHEN ISNULL(@confirmationCode,'') = '' THEN 1  \n"
                               + "				 WHEN ISNUMERIC(@confirmationCode) = 1 AND CAST(@confirmationCode AS INT) = b.leistacc THEN 1 \n"
                               + "				 WHEN CHARINDEX(@confirmationCode, b.string1) > 0 THEN 1  \n"
                               + "			ELSE 0 END = 1 AND b.buchstatus <> 0    \n"
                               + "	ELSE   \n"
                               + "		INSERT INTO @Resevs(ResId, ptyp)  \n"
                               + "		SELECT DISTINCT b.leistacc, b.preistyp  \n"
                               + "		FROM buch AS b  \n"
                               + "		INNER JOIN kat AS k ON k.katnr = b.katnr AND k.grpkto <> 1  \n"
                               + "		INNER JOIN zimmer AS z ON z.zinr = b.zimmernr   \n"
                               + "		INNER JOIN kunden AS kd ON kd.kdnr = b.kundennr AND CHARINDEX(@room, kd.name1) > 0     \n"
                               + "		WHERE b.reschar < 2 AND (b.mpehotel = @mpeHotel OR @bAllHotels = 1) AND b.globdbis >= @dtFrom AND b.globdvon <= @dtTo AND  \n"
                               + "			((b.globbnr < 1) OR (b.globbnr > 0 AND b.umzdurch  =1)) AND   \n"
                               + "			CASE WHEN ISNULL(@confirmationCode,'') = '' THEN 1  \n"
                               + "				 WHEN ISNUMERIC(@confirmationCode) = 1 AND CAST(@confirmationCode AS INT) = b.leistacc THEN 1 \n"
                               + "				 WHEN CHARINDEX(@confirmationCode, b.string1) > 0 THEN 1  \n"
                               + "			ELSE 0 END = 1 AND b.buchstatus <> 0    \n"
                               + "  \n"
                               + "	INSERT INTO @exRes (leistacc, kd)    \n"
                               + "	SELECT DISTINCT i.lgbuchnr, i.kb   \n"
                               + "	FROM " + user + ".ifcdata i   \n"
                               + "	INNER JOIN @Resevs r ON r.ResId = i.lgbuchnr  \n"
                               + "	WHERE i.kb = 1		  \n"
                               + "  \n"
                               + "  \n"
                               + "	DECLARE @tmpBoards TABLE (Id INT IDENTITY(1,1), leistacc INT, Board VARCHAR(MAX))  \n"
                               + "  \n"
                               + "	SELECT @idx = 0, @tRec = DATEDIFF(d, @dtFrom, @dtTo)  \n"
                               + "	WHILE @idx <= @tRec  \n"
                               + "	BEGIN  \n"
                               + "      DELETE FROM @tmpBoards \n"
                               + "		INSERT INTO @tmpBoards(leistacc, Board)  \n"
                               + "		SELECT DISTINCT fin.ResId, fin.sort   \n"
                               + "		FROM (  \n"
                               + "			SELECT DISTINCT r.ResId, ISNULL(ba.short, s.short) sort  \n"
                               + "			FROM @Resevs r  \n"
                               + "			INNER JOIN  bucharr AS b ON r.ResId = b.lgbuchnr AND b.jdatum = 0  \n"
                               + "			INNER JOIN splittab AS s ON s.stabnr = b.arrange AND CHARINDEX('x', s.flags) < 1   \n"
                               + "			OUTER APPLY (  \n"
                               + "				SELECT DISTINCT s.short  \n"
                               + "				FROM bucharr AS ba  \n"
                               + "				INNER JOIN splittab AS s ON s.stabnr = ba.arrange AND CHARINDEX('x', s.flags) < 1   \n"
                               + "				WHERE ba.lgbuchnr = r.ResId AND ba.jdatum > 0 AND DATEADD(d,ba.jdatum - 2440588,'1970-01-01') = (@dtFrom+@idx)  \n"
                               + "			) ba  \n"
                               + "		) fin  \n"
                               + "		WHERE ISNULL(fin.sort,'') <> ''  \n"
                               + "      INSERT INTO @tmpBoards(leistacc, Board) \n"
                               + "      SELECT DISTINCT r.ResId, s.short \n"
                               + "	    FROM @Resevs r \n"
                               + "	    INNER JOIN ptyp AS p ON p.ptypnr = r.ptyp \n"
                               + "	    INNER JOIN splittab AS s ON s.stabnr = p.stab1 AND CHARINDEX('x', s.flags) < 1 \n"
                               + "	    LEFT OUTER JOIN @tmpBoards t ON t.leistacc = r.ResId \n"
                               + "	    WHERE t.leistacc IS NULL \n"
                               + "	    SELECT @jdx = MIN(ID), @jTRec = MAX(ID) FROM @tmpBoards  \n"
                               + "      WHILE @jdx <= @jTRec \n"
                               + "      BEGIN \n"
                               + "		    IF NOT EXISTS (SELECT 1   \n"
                               + "		               FROM @Boards b  \n"
                               + "		               INNER JOIN @tmpBoards t ON t.leistacc = b.leistacc AND t.Id = @jdx)  \n"
                               + "			    INSERT INTO @Boards (leistacc, BoardCode, BoardName)  \n"
                               + "			    SELECT DISTINCT t.leistacc, t.Board, t.Board   \n"
                               + "			    FROM @tmpBoards t  \n"
                               + "			    WHERE t.Id = @jdx  \n"
                               + "          ELSE \n"
                               + "		        UPDATE b SET b.BoardCode = b.BoardCode+','+brd.Board,  b.BoardName = b.BoardName+',+brd.Board'  \n"
                               + "		        FROM @Boards b  \n"
                               + "		        CROSS APPLY (  \n"
                               + "			        SELECT a.Board  \n"
                               + "			        FROM (	  \n"
                               + "				        SELECT CASE WHEN CHARINDEX(t.Board, b.BoardCode) < 1 THEN t.Board ELSE '' END Board  \n"
                               + "				        FROM @tmpBoards t  \n"
                               + "				        WHERE t.leistacc = b.leistacc AND t.Id = @jdx \n"
                               + "			        ) a  \n"
                               + "			        WHERE a.Board <> ''  \n"
                               + "		        ) brd   \n"
                               + "          SET @jdx = @jdx + 1\n"
                               + "      END \n"
                               + "	  \n"
                               + "		SET @idx = @idx + 1  \n"
                               + "	END  \n"
                               + "  \n"
                               + "	UPDATE @Boards SET BoardCode = CASE WHEN RIGHT(BoardCode,1) = ',' THEN SUBSTRING(BoardCode,1,LEN(BoardCode)-1) ELSE BoardCode END,  \n"
                               + "		BoardName = CASE WHEN RIGHT(BoardName,1) = ',' THEN SUBSTRING(BoardName,1,LEN(BoardName)-1) ELSE BoardName END  \n"
                               + "	  \n"
                               + "	INSERT INTO @Profiles(leistacc, ProfileNo, FirstName, LastName, Member, [Password], [Address], City, PostalCode, Country, birthdayDT, Email, Telephone,  \n"
                               + "				VIP, Benefits, NationalityCode, NatId, LoyaltyProgramId, LoyaltyLevelId, CardType)  \n"
                               + "	SELECT DISTINCT b.leistacc, k.kdnr ProfileNo, k.vorname FirstName, k.name1 LastName, k.member Member, k.passwd Password, k.strasse Address, k.ort City,  \n"
                               + "		k.plz PostalCode, k.land Country, k.gebdat birthdayDT, k.email Email, k.funktel Telephone, k.vip VIP, ISNULL(v.remindtx,'') Benefits,   \n"
                               + "		ISNULL(n.land,'') NationalityCode, k.nat NatId, ISNULL(l.prgid,-1) LoyaltyProgramId, ISNULL(l.llevelid,-1) LoyaltyLevelId, ISNULL(lv.short_text,'') CardType  \n"
                               + "	FROM buch AS b  \n"
                               + "	INNER JOIN @Resevs rs ON rs.ResId = b.leistacc  \n"
                               + "	INNER JOIN kunden AS k ON k.kdnr = b.kundennr        \n"
                               + "	LEFT OUTER JOIN vipcode AS v ON v.codenr = k.vip    \n"
                               + "	LEFT OUTER JOIN natcode AS n ON n.codenr = k.nat      \n"
                               + "	LEFT OUTER JOIN loyalcrd AS l ON l.kdnr = k.kdnr  \n"
                               + "	LEFT OUTER JOIN loylvl AS lv ON lv.nr = l.llevelid   \n"
                               + "  WHERE b.buchstatus <> 0  \n"
                               + "	UNION ALL  \n"
                               + "	SELECT r.leistacc, k.kdnr ProfileNo, k.vorname FirstName, k.name1 LastName, k.member Member, k.passwd Password, k.strasse Address, k.ort City,  \n"
                               + "		k.plz PostalCode, k.land Country, k.gebdat birthdayDT, k.email Email, k.funktel Telephone, k.vip VIP, ISNULL(v.remindtx,'') Benefits,   \n"
                               + "		ISNULL(n.land,'') NationalityCode, k.nat NatId, ISNULL(l.prgid,-1) LoyaltyProgramId, ISNULL(l.llevelid,-1) LoyaltyLevelId, ISNULL(lv.short_text,'') CardType  \n"
                               + "	FROM reslinkp AS r  \n"
                               + "	INNER JOIN @Resevs rs ON rs.ResId = r.leistacc  \n"
                               + "	INNER JOIN kunden AS k ON k.kdnr = r.kundennr    \n"
                               + "	LEFT OUTER JOIN natcode AS n ON n.codenr = k.nat     \n"
                               + "	LEFT OUTER JOIN vipcode AS v ON v.codenr = k.vip      \n"
                               + "	LEFT OUTER JOIN loyalcrd AS l ON l.kdnr = k.kdnr     \n"
                               + "	LEFT OUTER JOIN loylvl AS lv ON lv.nr = l.llevelid   \n"
                               + "	UNION ALL  \n"
                               + "	SELECT b.leistacc, -1 ProfileNo, b.vorname0 FirstName, b.ehepart LastName,'' Member, b.pass0 Password,'' Address,'' City,'' PostalCode,'' Country,   \n"
                               + "		b.geb0 birthdayDT, '' Email,'' Telephone,'' VIP,'' Benefits, ISNULL(n.land,'') NationalityCode, b.nat0 NatId, -1 LoyaltyProgramId, -1 LoyaltyLevelId,  \n"
                               + "		'' CardType  \n"
                               + "	FROM begl AS b  \n"
                               + "	INNER JOIN @Resevs r ON r.ResId = b.leistacc   \n"
                               + "	LEFT OUTER JOIN natcode AS n ON n.codenr = b.nat0   \n"
                               + "	WHERE ISNULL(b.ehepart,'') <> ''  \n"
                               + "	UNION ALL  \n"
                               + "	SELECT b.leistacc, -1 ProfileNo, b.vorname1 FirstName, b.kind1 LastName,'' Member, b.pass1 Password,'' Address,'' City,'' PostalCode,'' Country,   \n"
                               + "		b.geb1 birthdayDT, '' Email,'' Telephone,'' VIP,'' Benefits, ISNULL(n.land,'') NationalityCode, b.nat1 NatId, -1 LoyaltyProgramId, -1 LoyaltyLevelId,  \n"
                               + "		'' CardType  \n"
                               + "	FROM begl AS b  \n"
                               + "	INNER JOIN @Resevs r ON r.ResId = b.leistacc   \n"
                               + "	LEFT OUTER JOIN natcode AS n ON n.codenr = b.nat1   \n"
                               + "	WHERE ISNULL(b.kind1,'') <> ''  \n"
                               + "	UNION ALL  \n"
                               + "	SELECT b.leistacc, -1 ProfileNo, b.vorname2 FirstName, b.kind2 LastName,'' Member, b.pass2 Password,'' Address,'' City,'' PostalCode,'' Country,   \n"
                               + "		b.geb2 birthdayDT, '' Email,'' Telephone,'' VIP,'' Benefits, ISNULL(n.land,'') NationalityCode, b.nat2 NatId, -1 LoyaltyProgramId, -1 LoyaltyLevelId,  \n"
                               + "		'' CardType  \n"
                               + "	FROM begl AS b  \n"
                               + "	INNER JOIN @Resevs r ON r.ResId = b.leistacc   \n"
                               + "	LEFT OUTER JOIN natcode AS n ON n.codenr = b.nat2   \n"
                               + "	WHERE ISNULL(b.kind2,'') <> ''  \n"
                               + "	UNION ALL  \n"
                               + "	SELECT b.leistacc, -1 ProfileNo, b.vorname3 FirstName, b.kind3 LastName,'' Member, b.pass3 Password,'' Address,'' City,'' PostalCode,'' Country,   \n"
                               + "		b.geb3 birthdayDT, '' Email,'' Telephone,'' VIP,'' Benefits, ISNULL(n.land,'') NationalityCode, b.nat3 NatId, -1 LoyaltyProgramId, -1 LoyaltyLevelId,  \n"
                               + "		'' CardType  \n"
                               + "	FROM begl AS b  \n"
                               + "	INNER JOIN @Resevs r ON r.ResId = b.leistacc   \n"
                               + "	LEFT OUTER JOIN natcode AS n ON n.codenr = b.nat3   \n"
                               + "	WHERE ISNULL(b.kind3,'') <> ''  \n"
                               + "	UNION ALL  \n"
                               + "	SELECT b.leistacc, -1 ProfileNo, b.vorname4 FirstName, b.kind4 LastName,'' Member, b.pass4 Password,'' Address,'' City,'' PostalCode,'' Country,   \n"
                               + "		b.geb4 birthdayDT, '' Email,'' Telephone,'' VIP,'' Benefits, ISNULL(n.land,'') NationalityCode, b.nat4 NatId, -1 LoyaltyProgramId, -1 LoyaltyLevelId,  \n"
                               + "		'' CardType  \n"
                               + "	FROM begl AS b  \n"
                               + "	INNER JOIN @Resevs r ON r.ResId = b.leistacc   \n"
                               + "	LEFT OUTER JOIN natcode AS n ON n.codenr = b.nat4   \n"
                               + "	WHERE ISNULL(b.kind4,'') <> ''  \n"
                               + "	UNION ALL  \n"
                               + "	SELECT b.leistacc, -1 ProfileNo, b.vorname5 FirstName, b.kind5 LastName,'' Member, b.pass5 Password,'' Address,'' City,'' PostalCode,'' Country,   \n"
                               + "		b.geb5 birthdayDT, '' Email,'' Telephone,'' VIP,'' Benefits, ISNULL(n.land,'') NationalityCode, b.nat5 NatId, -1 LoyaltyProgramId, -1 LoyaltyLevelId,  \n"
                               + "		'' CardType  \n"
                               + "	FROM begl AS b  \n"
                               + "	INNER JOIN @Resevs r ON r.ResId = b.leistacc   \n"
                               + "	LEFT OUTER JOIN natcode AS n ON n.codenr = b.nat5   \n"
                               + "	WHERE ISNULL(b.kind5,'') <> ''  \n"
                               + "	UNION ALL  \n"
                               + "	SELECT b.leistacc, -1 ProfileNo, b.vorname6 FirstName, b.kind6 LastName,'' Member, b.pass6 Password,'' Address,'' City,'' PostalCode,'' Country,   \n"
                               + "		b.geb6 birthdayDT, '' Email,'' Telephone,'' VIP,'' Benefits, ISNULL(n.land,'') NationalityCode, b.nat6 NatId, -1 LoyaltyProgramId, -1 LoyaltyLevelId,  \n"
                               + "		'' CardType  \n"
                               + "	FROM begl AS b  \n"
                               + "	INNER JOIN @Resevs r ON r.ResId = b.leistacc   \n"
                               + "	LEFT OUTER JOIN natcode AS n ON n.codenr = b.nat6   \n"
                               + "	WHERE ISNULL(b.kind6,'') <> ''  \n"
                               + "  \n"
                               + "	IF EXISTS (SELECT 1 FROM sys.tables AS t WHERE t.name = 'hit_loyalty_kunden')  \n"
                               + "		INSERT INTO @Loyalty(Kdnr, ClassId, ClassName, fnbdiscount, AvailablePoints, ratebuy)  \n"
                               + "		SELECT p.ProfileNo Kdnr, hlc.id ClassId, hlc.name ClassName, hlc.fnbdiscount, hlk.points AvailablePoints, hlg.ratebuy  \n"
                               + "		FROM @Profiles p  \n"
                               + "		INNER JOIN hit_loyalty_kunden hlk ON hlk.kdnr = p.ProfileNo  \n"
                               + "		INNER JOIN hit_loyalty_generalrules hlg ON 1 = 1  \n"
                               + "		INNER JOIN hit_loyalty_classes hlc ON hlc.id = hlk.class  \n"
                               + "  \n"
                               + "	DECLARE @tmpFut TABLE (ID INT IDENTITY(1,1), kdnr INT, Short VARCHAR(10))  \n"
                               + "	INSERT INTO @tmpFut(kdnr,Short)  \n"
                               + "	SELECT DISTINCT p.ProfileNo, gf.short  \n"
                               + "	FROM @Profiles p  \n"
                               + "	INNER JOIN g2gfeat AS g ON g.ref = p.ProfileNo  \n"
                               + "	INNER JOIN gfeat AS gf ON gf.ref = g.featnr	  \n"
                               + "	  \n"
                               + "	SELECT @idx = MIN(ID), @tRec = MAX(ID) FROM @tmpFut  \n"
                               + "	WHILE @idx <= @tRec  \n"
                               + "	BEGIN  \n"
                               + "		IF NOT EXISTS (SELECT 1   \n"
                               + "					   FROM @Fetures f  \n"
                               + "					   INNER JOIN @tmpFut t ON t.kdnr = f.ProfileId AND t.ID = @idx)  \n"
                               + "			INSERT INTO @Fetures(ProfileId,Short)  \n"
                               + "			SELECT kdnr, Short  \n"
                               + "			FROM @tmpFut  \n"
                               + "			WHERE id = @idx  \n"
                               + "		ELSE  \n"
                               + "			UPDATE f SET f.Short = f.Short+','+t.Short  \n"
                               + "			FROM @Fetures f  \n"
                               + "			INNER JOIN @tmpFut t ON t.kdnr = f.ProfileId AND t.ID = @idx   \n"
                               + "	  \n"
                               + "		SET @idx = @idx + 1  \n"
                               + "	END  \n"
                               + "  \n"
                               + "	DECLARE @Result TABLE (ReservationCode VARCHAR(MAX), ReservationId INT, OriginalReservationId INT, RoomId INT, Room VARCHAR(MAX),  \n"
                               + "		ArrivalDate DATETIME,DepartureDate DATETIME,ProfileNo INT, FirstName VARCHAR(MAX),  \n"
                               + "		LastName VARCHAR(MAX), Member VARCHAR(MAX), Password VARCHAR(MAX), Address VARCHAR(MAX),City VARCHAR(MAX), PostalCode VARCHAR(MAX),  \n"
                               + "		Country VARCHAR(MAX), BirthdayDate DATETIME, Email VARCHAR(MAX), Telephone VARCHAR(MAX), VIP INT,Benefits VARCHAR(MAX),  \n"
                               + "		NationalityId INT, Adults INT, Children INT, Note1 VARCHAR(MAX), Note2 VARCHAR(MAX), CustomerId INT,   \n"
                               + "		IsSharer BIT, BoardCode VARCHAR(MAX), BoardName VARCHAR(MAX),NoPos INT, ClassId INT, ClassName VARCHAR(MAX), AvailablePoints INT,   \n"
                               + "		fnbdiscount INT,ratebuy INT, future VARCHAR(MAX), RoomTypeId INT, RoomType VARCHAR(MAX), BookedRoomTypeId INT, BookedRoomType VARCHAR(MAX),  \n"
                               + "		TravelAgentId INT, TravelAgent VARCHAR(MAX), CompanyId INT, CompanyName VARCHAR(MAX), GrpoupId INT, GroupName VARCHAR(MAX), SourceId INT,   \n"
                               + "		SourceName VARCHAR(MAX), mpehotel INT, ReservStatus INT, NationalityCode VARCHAR(MAX), LoyaltyProgramId INT, LoyaltyLevelId INT,  \n"
                               + "		RateCodeId INT, RateCodeDescr VARCHAR(MAX), CardType VARCHAR(MAX))  \n"
                               + "  \n"
                               + "	INSERT INTO @Result  \n"
                               + "	SELECT DISTINCT b.string1 ReservationCode, b.buchnr ReservationId, b.leistacc OriginalReservationId, z.zinr RoomId, z.ziname Room,  \n"
                               + "		b.globdvon arrivalDT, b.globdbis departureDT,p.ProfileNo, p.FirstName,  \n"
                               + "		p.LastName, p.Member, p.[Password], p.[Address], p.City, p.PostalCode,  \n"
                               + "		p.Country, p.birthdayDT, p.Email, p.Telephone, p.VIP, p.Benefits,  \n"
                               + "		ISNULL(p.NatId,-1)  NatId, CASE WHEN @dtFrom = @dtTo THEN b.anzerw+b.zbett ELSE ISNULL(v.anzerw+v.zbett, b.anzerw+b.zbett) END Adults,  \n"
                               + "      CASE WHEN @dtFrom = @dtTo THEN b.anzkin1+b.anzkin2+b.anzkin3+b.anzkin4 ELSE ISNULL(v.anzkin1+v.anzkin2+v.anzkin3+v.anzkin4,b.anzkin1+b.anzkin2+b.anzkin3+b.anzkin4) END Children,  \n"
                               + "		b.not1txt, b.not2txt, p.ProfileNo kdnr,   \n"
                               + "		CASE WHEN b.sharenr < 1 THEN 0  \n"
                               + "			 WHEN b.sharenr > 0 AND b.sharenr <> b.leistacc THEN 1  \n"
                               + "		ELSE 0 END IsSharer,   \n"
                               + "	  \n"
                               + "		ISNULL(br.BoardCode,'RR') BoardCode, ISNULL(br.BoardName,'RR') BoardName,ISNULL(e.kd,0) noPos,  \n"
                               + "		ISNULL(l.ClassId, -1) ClassId, ISNULL(l.ClassName,'') ClassName, ISNULL(l.AvailablePoints,0) AvailablePoints,   \n"
                               + "		ISNULL(l.fnbdiscount, 0) fnbdiscount, ISNULL(l.ratebuy, 0) ratebuy, ISNULL(f.Short,'') future,  \n"
                               + "		b.katnr RoomTypeId, kt.kat RoomType, b.orgkatnr BookedRoomTypeId, kto.kat BookedRoomType,  \n"
                               + "		b.reisenr TA_ID, ISNULL(ta.name1,'') TAName, b.firmennr CompanyId, ISNULL(com.name1,'') CompanyName,  \n"
                               + "		b.gruppennr GrpoupId, ISNULL(grp.name1,'') GroupName, b.sourcenr SourceId, ISNULL(src.name1,'') SourceName,  \n"
                               + "		b.mpehotel, b.buchstatus, p.NationalityCode,   \n"
                               + "		p.LoyaltyProgramId, p.LoyaltyLevelId,  \n"
                               + "		b.preistyp, CAST(ISNULL(pt.grp,-1) AS VARCHAR(10))+' - '+CAST(ISNULL(pt.ptypnr,-1) AS VARCHAR(10))+' '+pt.ptyp, p.CardType  \n"
                               + "  \n"
                               + "	FROM buch AS b  \n"
                               + "	INNER JOIN @Resevs r ON r.ResId = b.leistacc  \n"
                               + "	INNER JOIN zimmer AS z ON z.zinr = b.zimmernr  \n"
                               + "	INNER JOIN @Profiles p ON p.leistacc = b.leistacc  \n"
                               + "	LEFT OUTER JOIN @exRes e ON e.leistacc = r.ResId  \n"
                               + "	LEFT OUTER JOIN @Loyalty l ON l.Kdnr = p.ProfileNo  \n"
                               + "	LEFT OUTER JOIN @Boards br ON br.leistacc = b.leistacc  \n"
                               + "	LEFT OUTER JOIN @Fetures f ON f.ProfileId = p.ProfileNo  \n"
                               + "	INNER JOIN kat AS kt ON kt.katnr = b.katnr  \n"
                               + "	INNER JOIN kat AS kto ON kto.katnr = b.orgkatnr  \n"
                               + "	LEFT OUTER JOIN kunden AS ta ON ta.kdnr = b.reisenr  \n"
                               + "	LEFT OUTER JOIN kunden AS com ON com.kdnr = b.firmennr  \n"
                               + "	LEFT OUTER JOIN kunden AS grp ON grp.kdnr = b.gruppennr  \n"
                               + "	LEFT OUTER JOIN kunden AS src ON src.kdnr = b.sourcenr  \n"
                               + "	LEFT OUTER JOIN ptyp AS pt ON pt.ptypnr = b.preistyp  \n"
                               + "  LEFT OUTER JOIN varbuch AS v ON v.buchnr = b.leistacc AND v.datum = @dtFrom  \n"
                               + "	WHERE ((b.globbnr < 1) OR (b.globbnr > 0 AND b.umzdurch = 1)) AND b.buchstatus <> 0   \n"
                               + "	ORDER BY z.ziname  \n"
                               + "	  \n"
                               + "	SELECT @tRec = COUNT(*) FROM @Result  \n"
                               + "  \n"
                               + "	SELECT lst.*  \n"
                               + "	FROM (  \n"
                               + "		SELECT @tRec TotalRecs, ROW_NUMBER() OVER (ORDER BY r.Room ) Row_ID, *  \n"
                               + "		FROM @Result r  \n"
                               + "	) lst  \n"
                               + "	WHERE lst.Row_ID BETWEEN @start AND  @ends  \n"
                               + "END");
            try
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
                return SQLCreate.ToString();
            }
            catch (Exception ex1)
            {
                logger.Error(ex1.ToString());
                var b = ex1.Message;
                return "";
            }
        }

        /// <summary>
        /// Returns string with code from store procedure GetReservationInfoPeriodAllReservs
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private string retReservationInfoPeriodAllReserv(string user)
        {
            //bool procExists = false;
            StringBuilder SQLCreate = new StringBuilder();
            string sCommand = "CREATE ";
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
            //bool b2000 = false;
            using (connection)
            {
                connection.ConnectionString = connectString;
                connection.Open();

                SqlDataAdapter da = new SqlDataAdapter("IF NOT EXISTS(SELECT 1 FROM sysobjects WHERE type = 'P' and name = 'GetReservationInfoPeriodAllReservs') SELECT 0 nCnt ELSE SELECT 1 nCnt", connection);
                DataSet ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    if ((int)ds.Tables[0].Rows[0]["nCnt"] == 1)
                        sCommand = "ALTER ";
            }
            SQLCreate.Clear();
            SQLCreate.Append(sCommand + " PROCEDURE " + user + ".GetReservationInfoPeriodAllReservs  @confirmationCode VARCHAR(50) AS  \n"
                               + "BEGIN \n"
                               + " \n"
                               + "	DECLARE @idx INT, @tRec INT, @jdx INT, @jTRec INT, @dtFrom DATETIME, @dtTo DATETIME    \n"
                               + "    \n"
                               + "	DECLARE @Resevs TABLE (ResId INT, ptyp INT, globdvon DATETIME, globdbis DATETIME)    \n"
                               + "	DECLARE @exRes TABLE (leistacc INT, kd INT)     \n"
                               + "	DECLARE @Profiles TABLE (leistacc INT, ProfileNo INT,FirstName VARCHAR(50),LastName VARCHAR(80),Member VARCHAR(50),    \n"
                               + "		Password VARCHAR(15),Address VARCHAR(80),City VARCHAR(50),PostalCode VARCHAR(17),Country VARCHAR(80),    \n"
                               + "		birthdayDT DATETIME,Email VARCHAR(75),Telephone VARCHAR(50),VIP INT,Benefits VARCHAR(250),    \n"
                               + "		NationalityCode VARCHAR(80), NatId INT, LoyaltyProgramId INT, LoyaltyLevelId INT, CardType VARCHAR(200))    \n"
                               + "	DECLARE @Loyalty TABLE (Kdnr INT, ClassId INT, ClassName NVARCHAR(90), fnbdiscount INT, AvailablePoints INT, ratebuy INT)    \n"
                               + "	    \n"
                               + "	DECLARE @Boards TABLE (leistacc INT, BoardCode VARCHAR(MAX) , BoardName VARCHAR(MAX), UNIQUE NONCLUSTERED(leistacc))    \n"
                               + "	DECLARE @Fetures TABLE (ProfileId INT, Short VARCHAR(MAX))    \n"
                               + "	    \n"
                               + "	IF ISNULL(@confirmationCode,'') = ''   \n"
                               + "		SET @confirmationCode = 'no reservation selected for search' \n"
                               + "	 \n"
                               + "	INSERT INTO @Resevs(ResId, ptyp, globdvon, globdbis)    \n"
                               + "	SELECT DISTINCT b.leistacc, b.preistyp, b.globdvon, b.globdbis \n"
                               + "	FROM buch AS b    \n"
                               + "	WHERE b.reschar < 2 AND ((b.globbnr < 1) OR (b.globbnr > 0 AND b.umzdurch  =1)) AND  LTRIM(RTRIM(b.string1)) = LTRIM(RTRIM(@confirmationCode)) \n"
                               + "    \n"
                               + "	INSERT INTO @exRes (leistacc, kd)      \n"
                               + "	SELECT DISTINCT i.lgbuchnr, i.kb     \n"
                               + "	FROM ifcdata i     \n"
                               + "	INNER JOIN @Resevs r ON r.ResId = i.lgbuchnr    \n"
                               + "	WHERE i.kb = 1		    \n"
                               + "    \n"
                               + "    \n"
                               + "	DECLARE @tmpBoards TABLE (Id INT IDENTITY(1,1), leistacc INT, Board VARCHAR(MAX))    \n"
                               + "	SELECT @dtFrom = MIN(globdvon), @dtTo = MAX(globdbis) FROM @Resevs    \n"
                               + "    \n"
                               + "	SELECT @idx = 0, @tRec = DATEDIFF(d, @dtFrom, @dtTo)    \n"
                               + "	WHILE @idx <= @tRec    \n"
                               + "	BEGIN    \n"
                               + "      DELETE FROM @tmpBoards   \n"
                               + "		INSERT INTO @tmpBoards(leistacc, Board)    \n"
                               + "		SELECT DISTINCT fin.ResId, fin.sort     \n"
                               + "		FROM (    \n"
                               + "			SELECT DISTINCT r.ResId, ISNULL(ba.short, s.short) sort    \n"
                               + "			FROM @Resevs r    \n"
                               + "			INNER JOIN  bucharr AS b ON r.ResId = b.lgbuchnr AND b.jdatum = 0    \n"
                               + "			INNER JOIN splittab AS s ON s.stabnr = b.arrange AND CHARINDEX('x', s.flags) < 1     \n"
                               + "			OUTER APPLY (    \n"
                               + "				SELECT DISTINCT s.short    \n"
                               + "				FROM bucharr AS ba    \n"
                               + "				INNER JOIN splittab AS s ON s.stabnr = ba.arrange AND CHARINDEX('x', s.flags) < 1     \n"
                               + "				WHERE ba.lgbuchnr = r.ResId AND ba.jdatum > 0 AND DATEADD(d,ba.jdatum - 2440588,'1970-01-01') = (@dtFrom+@idx)    \n"
                               + "			) ba    \n"
                               + "		) fin    \n"
                               + "		WHERE ISNULL(fin.sort,'') <> ''    \n"
                               + "      INSERT INTO @tmpBoards(leistacc, Board)   \n"
                               + "      SELECT DISTINCT r.ResId, s.short   \n"
                               + "	    FROM @Resevs r   \n"
                               + "	    INNER JOIN ptyp AS p ON p.ptypnr = r.ptyp   \n"
                               + "	    INNER JOIN splittab AS s ON s.stabnr = p.stab1 AND CHARINDEX('x', s.flags) < 1   \n"
                               + "	    LEFT OUTER JOIN @tmpBoards t ON t.leistacc = r.ResId   \n"
                               + "	    WHERE t.leistacc IS NULL   \n"
                               + "	    SELECT @jdx = MIN(ID), @jTRec = MAX(ID) FROM @tmpBoards    \n"
                               + "      WHILE @jdx <= @jTRec   \n"
                               + "      BEGIN   \n"
                               + "		    IF NOT EXISTS (SELECT 1     \n"
                               + "		               FROM @Boards b    \n"
                               + "		               INNER JOIN @tmpBoards t ON t.leistacc = b.leistacc AND t.Id = @jdx)    \n"
                               + "			    INSERT INTO @Boards (leistacc, BoardCode, BoardName)    \n"
                               + "			    SELECT DISTINCT t.leistacc, t.Board, t.Board     \n"
                               + "			    FROM @tmpBoards t    \n"
                               + "			    WHERE t.Id = @jdx    \n"
                               + "          ELSE   \n"
                               + "		        UPDATE b SET b.BoardCode = b.BoardCode+','+brd.Board,  b.BoardName = b.BoardName+',+brd.Board'    \n"
                               + "		        FROM @Boards b    \n"
                               + "		        CROSS APPLY (    \n"
                               + "			        SELECT a.Board    \n"
                               + "			        FROM (	    \n"
                               + "				        SELECT CASE WHEN CHARINDEX(t.Board, b.BoardCode) < 1 THEN t.Board ELSE '' END Board    \n"
                               + "				        FROM @tmpBoards t    \n"
                               + "				        WHERE t.leistacc = b.leistacc AND t.Id = @jdx   \n"
                               + "			        ) a    \n"
                               + "			        WHERE a.Board <> ''    \n"
                               + "		        ) brd     \n"
                               + "          SET @jdx = @jdx + 1  \n"
                               + "      END   \n"
                               + "	    \n"
                               + "		SET @idx = @idx + 1    \n"
                               + "	END    \n"
                               + "    \n"
                               + "	UPDATE @Boards SET BoardCode = CASE WHEN RIGHT(BoardCode,1) = ',' THEN SUBSTRING(BoardCode,1,LEN(BoardCode)-1) ELSE BoardCode END,    \n"
                               + "		BoardName = CASE WHEN RIGHT(BoardName,1) = ',' THEN SUBSTRING(BoardName,1,LEN(BoardName)-1) ELSE BoardName END    \n"
                               + "	    \n"
                               + "	INSERT INTO @Profiles(leistacc, ProfileNo, FirstName, LastName, Member, [Password], [Address], City, PostalCode, Country, birthdayDT, Email, Telephone,    \n"
                               + "				VIP, Benefits, NationalityCode, NatId, LoyaltyProgramId, LoyaltyLevelId, CardType)    \n"
                               + "	SELECT DISTINCT b.leistacc, k.kdnr ProfileNo, k.vorname FirstName, k.name1 LastName, k.member Member, k.passwd Password, k.strasse Address, k.ort City,    \n"
                               + "		k.plz PostalCode, k.land Country, k.gebdat birthdayDT, k.email Email, k.funktel Telephone, k.vip VIP, ISNULL(v.remindtx,'') Benefits,     \n"
                               + "		ISNULL(n.land,'') NationalityCode, k.nat NatId, ISNULL(l.prgid,-1) LoyaltyProgramId, ISNULL(l.llevelid,-1) LoyaltyLevelId, ISNULL(lv.short_text,'') CardType    \n"
                               + "	FROM buch AS b    \n"
                               + "	INNER JOIN @Resevs rs ON rs.ResId = b.leistacc    \n"
                               + "	INNER JOIN kunden AS k ON k.kdnr = b.kundennr          \n"
                               + "	LEFT OUTER JOIN vipcode AS v ON v.codenr = k.vip      \n"
                               + "	LEFT OUTER JOIN natcode AS n ON n.codenr = k.nat        \n"
                               + "	LEFT OUTER JOIN loyalcrd AS l ON l.kdnr = k.kdnr    \n"
                               + "	LEFT OUTER JOIN loylvl AS lv ON lv.nr = l.llevelid     \n"
                               + "	UNION ALL    \n"
                               + "	SELECT r.leistacc, k.kdnr ProfileNo, k.vorname FirstName, k.name1 LastName, k.member Member, k.passwd Password, k.strasse Address, k.ort City,    \n"
                               + "		k.plz PostalCode, k.land Country, k.gebdat birthdayDT, k.email Email, k.funktel Telephone, k.vip VIP, ISNULL(v.remindtx,'') Benefits,     \n"
                               + "		ISNULL(n.land,'') NationalityCode, k.nat NatId, ISNULL(l.prgid,-1) LoyaltyProgramId, ISNULL(l.llevelid,-1) LoyaltyLevelId, ISNULL(lv.short_text,'') CardType    \n"
                               + "	FROM reslinkp AS r    \n"
                               + "	INNER JOIN @Resevs rs ON rs.ResId = r.leistacc    \n"
                               + "	INNER JOIN kunden AS k ON k.kdnr = r.kundennr      \n"
                               + "	LEFT OUTER JOIN natcode AS n ON n.codenr = k.nat       \n"
                               + "	LEFT OUTER JOIN vipcode AS v ON v.codenr = k.vip        \n"
                               + "	LEFT OUTER JOIN loyalcrd AS l ON l.kdnr = k.kdnr       \n"
                               + "	LEFT OUTER JOIN loylvl AS lv ON lv.nr = l.llevelid     \n"
                               + "	UNION ALL    \n"
                               + "	SELECT b.leistacc, -1 ProfileNo, b.vorname0 FirstName, b.ehepart LastName,'' Member, b.pass0 Password,'' Address,'' City,'' PostalCode,'' Country,     \n"
                               + "		b.geb0 birthdayDT, '' Email,'' Telephone,'' VIP,'' Benefits, ISNULL(n.land,'') NationalityCode, b.nat0 NatId, -1 LoyaltyProgramId, -1 LoyaltyLevelId,    \n"
                               + "		'' CardType    \n"
                               + "	FROM begl AS b    \n"
                               + "	INNER JOIN @Resevs r ON r.ResId = b.leistacc     \n"
                               + "	LEFT OUTER JOIN natcode AS n ON n.codenr = b.nat0     \n"
                               + "	WHERE ISNULL(b.ehepart,'') <> ''    \n"
                               + "	UNION ALL    \n"
                               + "	SELECT b.leistacc, -1 ProfileNo, b.vorname1 FirstName, b.kind1 LastName,'' Member, b.pass1 Password,'' Address,'' City,'' PostalCode,'' Country,     \n"
                               + "		b.geb1 birthdayDT, '' Email,'' Telephone,'' VIP,'' Benefits, ISNULL(n.land,'') NationalityCode, b.nat1 NatId, -1 LoyaltyProgramId, -1 LoyaltyLevelId,    \n"
                               + "		'' CardType    \n"
                               + "	FROM begl AS b    \n"
                               + "	INNER JOIN @Resevs r ON r.ResId = b.leistacc     \n"
                               + "	LEFT OUTER JOIN natcode AS n ON n.codenr = b.nat1     \n"
                               + "	WHERE ISNULL(b.kind1,'') <> ''    \n"
                               + "	UNION ALL    \n"
                               + "	SELECT b.leistacc, -1 ProfileNo, b.vorname2 FirstName, b.kind2 LastName,'' Member, b.pass2 Password,'' Address,'' City,'' PostalCode,'' Country,     \n"
                               + "		b.geb2 birthdayDT, '' Email,'' Telephone,'' VIP,'' Benefits, ISNULL(n.land,'') NationalityCode, b.nat2 NatId, -1 LoyaltyProgramId, -1 LoyaltyLevelId,    \n"
                               + "		'' CardType    \n"
                               + "	FROM begl AS b    \n"
                               + "	INNER JOIN @Resevs r ON r.ResId = b.leistacc     \n"
                               + "	LEFT OUTER JOIN natcode AS n ON n.codenr = b.nat2     \n"
                               + "	WHERE ISNULL(b.kind2,'') <> ''    \n"
                               + "	UNION ALL    \n"
                               + "	SELECT b.leistacc, -1 ProfileNo, b.vorname3 FirstName, b.kind3 LastName,'' Member, b.pass3 Password,'' Address,'' City,'' PostalCode,'' Country,     \n"
                               + "		b.geb3 birthdayDT, '' Email,'' Telephone,'' VIP,'' Benefits, ISNULL(n.land,'') NationalityCode, b.nat3 NatId, -1 LoyaltyProgramId, -1 LoyaltyLevelId,    \n"
                               + "		'' CardType    \n"
                               + "	FROM begl AS b    \n"
                               + "	INNER JOIN @Resevs r ON r.ResId = b.leistacc     \n"
                               + "	LEFT OUTER JOIN natcode AS n ON n.codenr = b.nat3     \n"
                               + "	WHERE ISNULL(b.kind3,'') <> ''    \n"
                               + "	UNION ALL    \n"
                               + "	SELECT b.leistacc, -1 ProfileNo, b.vorname4 FirstName, b.kind4 LastName,'' Member, b.pass4 Password,'' Address,'' City,'' PostalCode,'' Country,     \n"
                               + "		b.geb4 birthdayDT, '' Email,'' Telephone,'' VIP,'' Benefits, ISNULL(n.land,'') NationalityCode, b.nat4 NatId, -1 LoyaltyProgramId, -1 LoyaltyLevelId,    \n"
                               + "		'' CardType    \n"
                               + "	FROM begl AS b    \n"
                               + "	INNER JOIN @Resevs r ON r.ResId = b.leistacc     \n"
                               + "	LEFT OUTER JOIN natcode AS n ON n.codenr = b.nat4     \n"
                               + "	WHERE ISNULL(b.kind4,'') <> ''    \n"
                               + "	UNION ALL    \n"
                               + "	SELECT b.leistacc, -1 ProfileNo, b.vorname5 FirstName, b.kind5 LastName,'' Member, b.pass5 Password,'' Address,'' City,'' PostalCode,'' Country,     \n"
                               + "		b.geb5 birthdayDT, '' Email,'' Telephone,'' VIP,'' Benefits, ISNULL(n.land,'') NationalityCode, b.nat5 NatId, -1 LoyaltyProgramId, -1 LoyaltyLevelId,    \n"
                               + "		'' CardType    \n"
                               + "	FROM begl AS b    \n"
                               + "	INNER JOIN @Resevs r ON r.ResId = b.leistacc     \n"
                               + "	LEFT OUTER JOIN natcode AS n ON n.codenr = b.nat5     \n"
                               + "	WHERE ISNULL(b.kind5,'') <> ''    \n"
                               + "	UNION ALL    \n"
                               + "	SELECT b.leistacc, -1 ProfileNo, b.vorname6 FirstName, b.kind6 LastName,'' Member, b.pass6 Password,'' Address,'' City,'' PostalCode,'' Country,     \n"
                               + "		b.geb6 birthdayDT, '' Email,'' Telephone,'' VIP,'' Benefits, ISNULL(n.land,'') NationalityCode, b.nat6 NatId, -1 LoyaltyProgramId, -1 LoyaltyLevelId,    \n"
                               + "		'' CardType    \n"
                               + "	FROM begl AS b    \n"
                               + "	INNER JOIN @Resevs r ON r.ResId = b.leistacc     \n"
                               + "	LEFT OUTER JOIN natcode AS n ON n.codenr = b.nat6     \n"
                               + "	WHERE ISNULL(b.kind6,'') <> ''    \n"
                               + "    \n"
                               + "	IF EXISTS (SELECT 1 FROM sys.tables AS t WHERE t.name = 'hit_loyalty_kunden')    \n"
                               + "		INSERT INTO @Loyalty(Kdnr, ClassId, ClassName, fnbdiscount, AvailablePoints, ratebuy)    \n"
                               + "		SELECT p.ProfileNo Kdnr, hlc.id ClassId, hlc.name ClassName, hlc.fnbdiscount, hlk.points AvailablePoints, hlg.ratebuy    \n"
                               + "		FROM @Profiles p    \n"
                               + "		INNER JOIN hit_loyalty_kunden hlk ON hlk.kdnr = p.ProfileNo    \n"
                               + "		INNER JOIN hit_loyalty_generalrules hlg ON 1 = 1    \n"
                               + "		INNER JOIN hit_loyalty_classes hlc ON hlc.id = hlk.class    \n"
                               + "    \n"
                               + "	DECLARE @tmpFut TABLE (ID INT IDENTITY(1,1), kdnr INT, Short VARCHAR(10))    \n"
                               + "	INSERT INTO @tmpFut(kdnr,Short)    \n"
                               + "	SELECT DISTINCT p.ProfileNo, gf.short    \n"
                               + "	FROM @Profiles p    \n"
                               + "	INNER JOIN g2gfeat AS g ON g.ref = p.ProfileNo    \n"
                               + "	INNER JOIN gfeat AS gf ON gf.ref = g.featnr	    \n"
                               + "	    \n"
                               + "	SELECT @idx = MIN(ID), @tRec = MAX(ID) FROM @tmpFut    \n"
                               + "	WHILE @idx <= @tRec    \n"
                               + "	BEGIN    \n"
                               + "		IF NOT EXISTS (SELECT 1     \n"
                               + "					   FROM @Fetures f    \n"
                               + "					   INNER JOIN @tmpFut t ON t.kdnr = f.ProfileId AND t.ID = @idx)    \n"
                               + "			INSERT INTO @Fetures(ProfileId,Short)    \n"
                               + "			SELECT kdnr, Short    \n"
                               + "			FROM @tmpFut    \n"
                               + "			WHERE id = @idx    \n"
                               + "		ELSE    \n"
                               + "			UPDATE f SET f.Short = f.Short+','+t.Short    \n"
                               + "			FROM @Fetures f    \n"
                               + "			INNER JOIN @tmpFut t ON t.kdnr = f.ProfileId AND t.ID = @idx     \n"
                               + "	    \n"
                               + "		SET @idx = @idx + 1    \n"
                               + "	END    \n"
                               + "    \n"
                               + "	DECLARE @Result TABLE (ReservationCode VARCHAR(MAX), ReservationId INT, OriginalReservationId INT, RoomId INT, Room VARCHAR(MAX),    \n"
                               + "		ArrivalDate DATETIME,DepartureDate DATETIME,ProfileNo INT, FirstName VARCHAR(MAX),    \n"
                               + "		LastName VARCHAR(MAX), Member VARCHAR(MAX), Password VARCHAR(MAX), Address VARCHAR(MAX),City VARCHAR(MAX), PostalCode VARCHAR(MAX),    \n"
                               + "		Country VARCHAR(MAX), BirthdayDate DATETIME, Email VARCHAR(MAX), Telephone VARCHAR(MAX), VIP INT,Benefits VARCHAR(MAX),    \n"
                               + "		NationalityId INT, Adults INT, Children INT, Note1 VARCHAR(MAX), Note2 VARCHAR(MAX), CustomerId INT,     \n"
                               + "		IsSharer BIT, BoardCode VARCHAR(MAX), BoardName VARCHAR(MAX),NoPos INT, ClassId INT, ClassName VARCHAR(MAX), AvailablePoints INT,     \n"
                               + "		fnbdiscount INT,ratebuy INT, future VARCHAR(MAX), RoomTypeId INT, RoomType VARCHAR(MAX), BookedRoomTypeId INT, BookedRoomType VARCHAR(MAX),    \n"
                               + "		TravelAgentId INT, TravelAgent VARCHAR(MAX), CompanyId INT, CompanyName VARCHAR(MAX), GrpoupId INT, GroupName VARCHAR(MAX), SourceId INT,     \n"
                               + "		SourceName VARCHAR(MAX), mpehotel INT, ReservStatus INT, NationalityCode VARCHAR(MAX), LoyaltyProgramId INT, LoyaltyLevelId INT,    \n"
                               + "		RateCodeId INT, RateCodeDescr VARCHAR(MAX), CardType VARCHAR(MAX))    \n"
                               + "    \n"
                               + "	INSERT INTO @Result    \n"
                               + "	SELECT DISTINCT b.string1 ReservationCode, b.buchnr ReservationId, b.leistacc OriginalReservationId, ISNULL(z.zinr,-10) RoomId, ISNULL(z.ziname,'') Room,    \n"
                               + "		b.globdvon arrivalDT, b.globdbis departureDT,p.ProfileNo, p.FirstName,    \n"
                               + "		p.LastName, p.Member, p.[Password], p.[Address], p.City, p.PostalCode,    \n"
                               + "		p.Country, p.birthdayDT, p.Email, p.Telephone, p.VIP, p.Benefits,    \n"
                               + "		ISNULL(p.NatId,-1)  NatId, CASE WHEN @dtFrom = @dtTo THEN b.anzerw+b.zbett ELSE ISNULL(v.anzerw+v.zbett, b.anzerw+b.zbett) END Adults,    \n"
                               + "      CASE WHEN @dtFrom = @dtTo THEN b.anzkin1+b.anzkin2+b.anzkin3+b.anzkin4 ELSE ISNULL(v.anzkin1+v.anzkin2+v.anzkin3+v.anzkin4,b.anzkin1+b.anzkin2+b.anzkin3+b.anzkin4) END Children,    \n"
                               + "		b.not1txt, b.not2txt, p.ProfileNo kdnr,     \n"
                               + "		CASE WHEN b.sharenr < 1 THEN 0    \n"
                               + "			 WHEN b.sharenr > 0 AND b.sharenr <> b.leistacc THEN 1    \n"
                               + "		ELSE 0 END IsSharer,     \n"
                               + "	    \n"
                               + "		ISNULL(br.BoardCode,'RR') BoardCode, ISNULL(br.BoardName,'RR') BoardName,ISNULL(e.kd,0) noPos,    \n"
                               + "		ISNULL(l.ClassId, -1) ClassId, ISNULL(l.ClassName,'') ClassName, ISNULL(l.AvailablePoints,0) AvailablePoints,     \n"
                               + "		ISNULL(l.fnbdiscount, 0) fnbdiscount, ISNULL(l.ratebuy, 0) ratebuy, ISNULL(f.Short,'') future,    \n"
                               + "		b.katnr RoomTypeId, kt.kat RoomType, b.orgkatnr BookedRoomTypeId, kto.kat BookedRoomType,    \n"
                               + "		b.reisenr TA_ID, ISNULL(ta.name1,'') TAName, b.firmennr CompanyId, ISNULL(com.name1,'') CompanyName,    \n"
                               + "		b.gruppennr GrpoupId, ISNULL(grp.name1,'') GroupName, b.sourcenr SourceId, ISNULL(src.name1,'') SourceName,    \n"
                               + "		b.mpehotel, b.buchstatus, p.NationalityCode,     \n"
                               + "		p.LoyaltyProgramId, p.LoyaltyLevelId,    \n"
                               + "		b.preistyp, CAST(ISNULL(pt.grp,-1) AS VARCHAR(10))+' - '+CAST(ISNULL(pt.ptypnr,-1) AS VARCHAR(10))+' '+pt.ptyp, p.CardType    \n"
                               + "    \n"
                               + "	FROM buch AS b    \n"
                               + "	INNER JOIN @Resevs r ON r.ResId = b.leistacc    \n"
                               + "	LEFT OUTER JOIN zimmer AS z ON z.zinr = b.zimmernr    \n"
                               + "	INNER JOIN @Profiles p ON p.leistacc = b.leistacc    \n"
                               + "	LEFT OUTER JOIN @exRes e ON e.leistacc = r.ResId    \n"
                               + "	LEFT OUTER JOIN @Loyalty l ON l.Kdnr = p.ProfileNo    \n"
                               + "	LEFT OUTER JOIN @Boards br ON br.leistacc = b.leistacc    \n"
                               + "	LEFT OUTER JOIN @Fetures f ON f.ProfileId = p.ProfileNo    \n"
                               + "	INNER JOIN kat AS kt ON kt.katnr = b.katnr    \n"
                               + "	INNER JOIN kat AS kto ON kto.katnr = b.orgkatnr    \n"
                               + "	LEFT OUTER JOIN kunden AS ta ON ta.kdnr = b.reisenr    \n"
                               + "	LEFT OUTER JOIN kunden AS com ON com.kdnr = b.firmennr    \n"
                               + "	LEFT OUTER JOIN kunden AS grp ON grp.kdnr = b.gruppennr    \n"
                               + "	LEFT OUTER JOIN kunden AS src ON src.kdnr = b.sourcenr    \n"
                               + "	LEFT OUTER JOIN ptyp AS pt ON pt.ptypnr = b.preistyp    \n"
                               + "	LEFT OUTER JOIN varbuch AS v ON v.buchnr = b.leistacc AND v.datum = @dtFrom    \n"
                               + "	WHERE ((b.globbnr < 1) OR (b.globbnr > 0 AND b.umzdurch = 1))      \n"
                               + "	ORDER BY ISNULL(z.ziname,'')    \n"
                               + "	    \n"
                               + "	SELECT @tRec = COUNT(*) FROM @Result    \n"
                               + "    \n"
                               + "	SELECT lst.*    \n"
                               + "	FROM (    \n"
                               + "		SELECT @tRec TotalRecs, ROW_NUMBER() OVER (ORDER BY r.Room ) Row_ID, *    \n"
                               + "		FROM @Result r    \n"
                               + "	) lst    \n"
                               + " \n"
                               + "END");
            try
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
                return SQLCreate.ToString();
            }
            catch (Exception ex1)
            {
                logger.Error(ex1.ToString());
                var b = ex1.Message;
                return "";
            }
        }


        /// <summary>
        /// create sp 'GetReservationInfo2' to protel DB to get reservation info, hotel's customers
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private string retReservationInfo(string user)
		{
			//bool procExists = false;
			StringBuilder SQLCreate = new StringBuilder();
			string sCommand = "CREATE ";
			if (connection.State == ConnectionState.Open)
			{
				connection.Close();
			}
			//bool b2000 = false;
			using (connection)
			{
				connection.ConnectionString = connectString;
				connection.Open();

				SqlDataAdapter da = new SqlDataAdapter("IF NOT EXISTS(SELECT 1 FROM sysobjects WHERE type = 'P' and name = 'GetReservationInfo2') SELECT 0 nCnt ELSE SELECT 1 nCnt", connection);
				DataSet ds = new DataSet();
				da.Fill(ds);
				if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
					if ((int)ds.Tables[0].Rows[0]["nCnt"] == 1)
						sCommand = "ALTER ";
			}
			SQLCreate.Clear();
            SQLCreate.Append(sCommand + " PROCEDURE " + user + ".GetReservationInfo2 @confirmationCode VARCHAR(50), @room VARCHAR(10), @mpeHotel INT, @pageNo INT, @pageSize INT, @bAllHotels BIT AS	 \n"
                           + "BEGIN \n"
                           + "	DECLARE @pDate DATETIME \n"
                           + "	SELECT @pDate = pdate FROM datum AS d WHERE d.mpehotel = @mpeHotel \n"
                           + "	EXEC " + user + ".GetReservationInfoPeriod @confirmationCode,@pDate, @pDate, @room,@mpeHotel, @pageNo, @pageSize, @bAllHotels \n"
                           + "END");
			try
			{
				if (connection.State == ConnectionState.Open)
				{
					connection.Close();
				}
				return SQLCreate.ToString();
			}
			catch (Exception ex1)
			{
				logger.Error(ex1.ToString());
				var b = ex1.Message;
				return "";
			}
		}

		/// <summary>
		/// create sp 'GetReservationInfo' to ermis DB to get reservation info, hotel's customers
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		private string retErmisResInfo(string user)
		{
            return "DECLARE @type VARCHAR(100)   \n"
               + "IF NOT EXISTS (SELECT 1 FROM sys.all_objects ao WHERE ao.name = 'GetReservationInfo' AND ao.[type] = 'P')   \n"
               + "SET @type = 'CREATE '   \n"
               + " ELSE   \n"
               + "SET @type = 'ALTER '   \n"
               + "BEGIN    \n"
               + "	DECLARE @SQL NVARCHAR(MAX)   \n"
               + "	SET @SQL = @type + '   \n"
               + "PROCEDURE " + user + ".GetReservationInfo @confirmationCode VARCHAR(50), @room VARCHAR(100),  @pageNo INT, @pageSize INT AS    \n"
               + "BEGIN     \n"
               + "DECLARE @totRec INT, @start INT, @ends INT    \n"
               + "SELECT @totRec = SUM(a.bekrapri)     \n"
               + "FROM (     \n"
               + "	SELECT COUNT(b.bekrapri) bekrapri       \n"
               + "	FROM " + user + ".bekra b     \n"
               + "	INNER JOIN " + user + ".bereg br ON br.berekra = b.bekrapri     \n"
               + "	WHERE b.bekrareg = ''5'' AND b.bekrakat <> ''S''      \n"
               + "	UNION ALL     \n"
               + "	SELECT COUNT(b.belonum)     \n"
               + "	FROM " + user + ".belog b     \n"
               + "	WHERE ISNULL(b.belokra,0) = 0 AND ISNULL(b.belofl2,'''') = ''''      \n"
               + ") a     \n"
               + "SET @start = CASE WHEN @pageNo < 0 THEN 1 ELSE (@pageSize * @pageNo) + 1 END    \n"
               + "SET @ends = CASE WHEN @pageNo < 0 THEN @totRec ELSE @pageSize * (@pageNo + 1) END    \n"
               + "SELECT *    \n"
               + "FROM (      \n"
               + "	SELECT @totRec TotalRecs, ROW_NUMBER() OVER (ORDER BY a.ReservationId) Row_ID,a.ReservationCode,   \n"
               + "			a.ReservationId, a.OriginalReservationId, -1 RoomId, a.Room, a.arrivalDT ArrivalDate, a.departureDT DepartureDate, a.ProfileNo, a.FirstName,   \n"
               + "			a.LastName,'''' Member,'''' [Password],a.[Address], a.City,'''' PostalCode, a.Country,    \n"
               + "			ISNULL(a.birthdayDT,''1900-01-01'') BirthdayDate, a.Email,'''' Telephone,    \n"
               + "			-1 VIP,'''' Benefits, a.NationalityCode NationalityId, a.Adults, a.Children,'''' Note1, '''' Note2,-1 CustomerId, a.IsSharer IsSharer,   \n"
               + "			a.BoardCode, a.BoardName, 0 NoPos,    \n"
               + "            -1 ClassId, '''' ClassName, 0 AvailablePoints, 0 fnbdiscount,0 ratebuy, '''' future, \n"
               + "            -1 RoomTypeId, a.beretype RoomType, -1 BookedRoomTypeId, a.beretype2 BookedRoomType, -1 TravelAgentId, '''' TravelAgent, \n"
               + "            -1 CompanyId, '''' CompanyName, -1 GrpoupId, '''' GroupName, -1 SourceId, '''' SourceName, 1 mpehotel, 1 ReservStatus, \n"
               + "             a.NationalityCode, -1 LoyaltyProgramId, -1 LoyaltyLevelId, a.bereTKey RateCodeId, a.beretim RateCodeDescr, '''' CardType  \n"
               + "	FROM (     \n"
               + "		SELECT ISNULL(b.bekraref,'''') ReservationCode, br.bereaaa ReservationId, br.bereaaa OriginalReservationId,    \n"
               + "			br.bereroom Room,      \n"
               + "			b.bekraarr arrivalDT, b.bekradep departureDT, br.bereaaa ProfileNo,     \n"
               + "			ISNULL(br.berenam,'''') FirstName, ISNULL(br.bereepo,'''') LastName,      \n"
               + "			ISNULL(br.bereadr1,'''')+'' ''+ISNULL(br.bereadr2,'''') ''Address'',      \n"
               + "			ISNULL(br.beretge,'''') City, ISNULL(br.bereeth, '''') Country, br.berehge birthdayDT,      \n"
               + "			ISNULL(br.bereemail,'''') Email,	ISNULL(br.beretge,'''') NationalityCode, b.bekrapax Adults,      \n"
               + "			ISNULL(b.bekrapaxc,0)+ISNULL(b.bekrapaxd,0)+ISNULL(b.bekrachc,0)+ISNULL(b.bekrachd,0) Children,     \n"
               + "			b.bekraboard BoardCode, b.bekraboard BoardName, b.bekraTKey RateId,    \n"
               + "			CASE WHEN ISNULL(b.bekrares1,'''') <> '''' THEN ''TRUE'' ELSE ''FALSE'' END IsSharer, \n"
               + "			br.beretype, br.beretype2, br.bereTKey, br.beretim     \n"
               + "		FROM " + user + ".bekra b     \n"
               + "		INNER JOIN " + user + ".bereg br ON br.berekra = b.bekrapri     \n"
               + "		WHERE b.bekrareg = ''5'' AND b.bekrakat <> ''S'' AND   \n"
               + "            (br.bereroom LIKE CASE WHEN ISNULL(@room,'''') <> '''' THEN @room+''%'' ELSE ''%'' END  OR br.bereepo+'' ''+br.berenam LIKE CASE WHEN ISNULL(@room, '''') <> '''' THEN ''%'' + @room + ''%'' ELSE ''%'' END)    \n"
               + " 			AND ISNULL(b.bekraref,'''') LIKE CASE WHEN ISNULL(@confirmationCode,'''') <> '''' THEN @confirmationCode+''%'' ELSE ''%'' END     \n"
               + "		UNION ALL     \n"
               + "		SELECT * FROM (     \n"
               + "			SELECT '''' ReservationCode, b.belonum ReservationId, b.belonum OriginalReservationId, CAST(7500+ROW_NUMBER() OVER (ORDER BY belonum) AS NVARCHAR(10)) Room,     \n"
               + "				bg.begendat arrivalDT, bg.begendat departureDT, b.belonum ProfileNo, ''-'' FirstName,     \n"
               + "				ISNULL(bp.bepraname, ISNULL(b.beloadt,'''')) LastName, ISNULL(bp.bepraadr,'''') ''Address'',     \n"
               + "				ISNULL(bp.bepracity,'''') City, ISNULL(bp.bepracoun,'''') Country, GETDATE() birthdayDT,     \n"
               + "				'''' Email, ISNULL(bp.bepracoun,'''') NationalityCode, 1 Adults, 0 Children, ''RR'' BoardCode, ''RR'' BoardName, 0 RateId,    \n"
               + "				''FALSE'' IsSharer , '''' beretype,'''' beretype2, -1 beretkey, '''' beretim     \n"
               + "			FROM " + user + ".belog b     \n"
               + "			INNER JOIN " + user + ".begen bg ON 1=1     \n"
               + "			LEFT OUTER JOIN " + user + ".bepra bp ON bp.bepracode = ISNULL(b.beloprakt,'''')     \n"
               + "			WHERE ISNULL(b.belokra,0) = 0 AND ISNULL(b.belofl2,'''') = ''''      \n"
               + "		) b     \n"
               + "		WHERE (b.Room LIKE CASE WHEN ISNULL(@room,'''') <> '''' THEN @room+''%'' ELSE ''%'' END OR b.ProfileNo LIKE  CASE WHEN ISNULL(@room,'''') <> '''' THEN ''%'' + @room + ''%'' ELSE ''%'' END)   \n"
               + "			AND ISNULL(b.ReservationCode,'''') LIKE CASE WHEN ISNULL(@confirmationCode,'''') <> '''' THEN @confirmationCode+''%'' ELSE ''%'' END     \n"
               + "	) a     \n"
               + ") lst     \n"
               + "WHERE lst.Row_ID BETWEEN @start AND @ends    \n"
               + "END'   \n"
               + "EXEC(@SQL)   \n"
               + "END";
        }

		/// <summary>
		/// create sp 'GetDepartments' to protel DB to get departments
		/// </summary>
		/// <param name="username">protel db's username</param>
		/// <returns></returns>
		private string retProtelDepartments(string username)
		{
			StringBuilder SQLCreate = new StringBuilder();
			string sCommand = "CREATE ";
			if (connection.State == ConnectionState.Open)
			{
				connection.Close();
			}
			//bool b2000 = false;
			using (connection)
			{
				connection.ConnectionString = connectString;
				connection.Open();

				SqlDataAdapter da = new SqlDataAdapter("IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE type = 'P' and name = 'GetDepartments') SELECT 0 nCnt ELSE SELECT 0 nCnt ", connection);
				DataSet ds = new DataSet();
				da.Fill(ds);
				if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
					if ((int)ds.Tables[0].Rows[0]["nCnt"] == 1)
						sCommand = "ALTER ";
			}

			return sCommand + " PROCEDURE " + username + ".GetDepartments @mpeHotel INT, @IS_ON_WEB INT  AS  \n"
				   + "	BEGIN  \n"
				   + "		SELECT u.ktonr DepartmentId, u.bez [Description], ISNULL(h.gruppe,'') GroupName, u.mpehotel HotelId, \n"
				   + "			Vat = m.satz  \n"
				   + "		FROM " + username + ".ukto u \n"
				   + "		LEFT OUTER JOIN " + username + ".hgruppen h ON h.hgnr = u.hauptgrp \n"
				   + "		INNER JOIN " + username + ".mwst m ON m.satznr = u.mwstnr \n"
				   + "		WHERE (u.mpehotel = @mpeHotel OR u.mpehotel = 0) AND u.inet = @IS_ON_WEB \n"
				   + "	END;";
		}

		/// <summary>
		/// Departments from PMS Ermis
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		private string retErmisDep(string username)
		{
			return "IF NOT EXISTS (SELECT 1 FROM sys.all_objects ao WHERE ao.name = 'GetDepartments' AND ao.[type] = 'P') \n"
		   + "		BEGIN  \n"
		   + "			DECLARE @SQL1 VARCHAR(4000) \n"
		   + "			SET @SQL1 = ' \n"
		   + "			CREATE PROCEDURE [" + username + "].[GetDepartments] AS  \n"
		   + "			SELECT lst.DepartmentId, lst.[Description], '''' GroupName, 0 HotelId,CAST(lst.Vat as DECIMAL(19,2)) Vat  \n"
		   + "			FROM (  \n"
		   + "				SELECT CAST(CAST(b.betmhkey AS VARCHAR(10))+''0'' AS INT) DepartmentId, b.betmhdescr+'' 0%'' [Description], 0 Vat  \n"
		   + "				FROM " + username + ".betmh b  \n"
		   + "				WHERE ISNULL(b.betmhdescr,'''') <> ''''  \n"
		   + "				UNION ALL  \n"
		   + "				SELECT CAST(CAST(b.betmhkey AS VARCHAR(10))+''1'' AS INT) DepartmentId, b.betmhdescr+'' ''+CAST(bg.begenf1 AS VARCHAR(10))+''%'' [Description], bg.begenf1 Vat  \n"
		   + "				FROM " + username + ".betmh b  \n"
		   + "				INNER JOIN " + username + ".begen bg ON ISNULL(bg.begenf1,0) <> 0   \n"
		   + "				WHERE ISNULL(b.betmhdescr,'''') <> ''''  \n"
		   + "				UNION ALL  \n"
		   + "				SELECT CAST(CAST(b.betmhkey AS VARCHAR(10))+''2'' AS INT) DepartmentId, b.betmhdescr+'' ''+CAST(bg.begenf2 AS VARCHAR(10))+''%'' [Description], bg.begenf2 Vat  \n"
		   + "				FROM " + username + ".betmh b  \n"
		   + "				INNER JOIN " + username + ".begen bg ON ISNULL(bg.begenf2,0) <> 0   \n"
		   + "				WHERE ISNULL(b.betmhdescr,'''') <> ''''  \n"
		   + "				UNION ALL  \n"
		   + "				SELECT CAST(CAST(b.betmhkey AS VARCHAR(10))+''3'' AS INT) DepartmentId, b.betmhdescr+'' ''+CAST(bg.begenf3 AS VARCHAR(10))+''%'' [Description], bg.begenf3 Vat  \n"
		   + "				FROM " + username + ".betmh b  \n"
		   + "				INNER JOIN " + username + ".begen bg ON ISNULL(bg.begenf3,0) <> 0   \n"
		   + "				WHERE ISNULL(b.betmhdescr,'''') <> ''''  \n"
		   + "			) lst  \n"
		   + "			ORDER BY lst.[Description]' \n"
		   + "			EXEC(@SQL1) \n"
		   + "		END;";
		}

		/// <summary>
		/// Gets Boards from PMS Protel
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public string retAllBoardsProtel(string username)
		{

			return "IF NOT EXISTS(SELECT ao.*  \n"
		   + "			  FROM sys.all_objects ao \n"
		   + "			  INNER JOIN sys.schemas s ON s.name = '" + username + "' AND ao.schema_id = s.schema_id  \n"
		   + "			  WHERE ao.type = 'P' AND ao.name = 'GetAllBoards') \n"
		   + "BEGIN 		 \n"
		   + "	DECLARE @SQL VARCHAR(8000) \n"
		   + "	SET @SQL = '	   \n"
		   + "	CREATE PROCEDURE '" + username + "'.GetAllBoards @all BIT AS \n"
		   + "	BEGIN  \n"
		   + "		SELECT CAST(r.ref AS FLOAT) BoardId, CASE WHEN @all = ''True'' THEN r.short ELSE NULL END Code, r.[text] BoardDescr   \n"
		   + "		FROM rategrp r  \n"
		   + "		WHERE CAST(ISNULL(r.inet,0) AS VARCHAR(10)) LIKE CASE WHEN @all = ''True'' THEN ''1'' ELSE ''%'' END \n"
		   + "		ORDER BY r.ref \n"
		   + "	END' \n"
		   + "	EXEC(@SQL) \n"
		   + "END";
		}

		/// <summary>
		/// Insert points into hit_loyalty_actions
		/// </summary>
		/// <param name="username"></param>
		/// <param name="kdnr"></param>
		/// <param name="actiondate"></param>
		/// <param name="type"></param>
		/// <param name="points"></param>
		/// <param name="notes"></param>
		/// <returns></returns>
		public bool InsertActions(string username, int? kdnr, DateTime? actiondate, int type, int points, string notes)
		{
			StringBuilder command = new StringBuilder();
			command.Clear();
			command.Append("INSERT INTO " + username + ".hit_loyalty_actions(kdnr, actiondate, type, points, notes) \n"
				   + "  VALUES (@kdnr, @actiondate, @type, @points, @notes)");

			return execSQLForLoyalty(command.ToString(), kdnr, actiondate, type, points, notes);
		}

		/// <summary>
		/// Check if GDPR Table hit_DeleteProfileFromPos Exists
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public int CheckIfTableExists()
		{
			int exists = 0;
			StringBuilder command = new StringBuilder();
			command.Clear();
			command.Append("IF EXISTS (SELECT 1 FROM sys.tables AS t WHERE t.name = 'hit_DeleteProfileFromPos') SELECT 1 ELSE SELECT 0 ");
			try
			{
				if (connection.State == ConnectionState.Open)
				{
					connection.Close();
				}

				using (connection)
				{
					connection.ConnectionString = connectString;
					connection.Open();
					SqlCommand comm = new SqlCommand(command.ToString(), connection);
					using (SqlDataReader reader = comm.ExecuteReader())
					{
						while (reader.Read())
						{
							exists = (int)reader.GetInt32(0);
						}
					}
					comm.ExecuteNonQuery();
				}
				return exists;
			}
			catch (Exception ex1)
			{
				logger.Error(ex1.ToString());
				var b = ex1.Message;
				return exists;
			}
		}

		/// <summary>
		/// GDPR Get CustomersId To Delete 
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public List<long> GetCustomersIds(string username)
		{
			List<long> custIds = new List<long>();
			StringBuilder command = new StringBuilder();
			command.Clear();
			command.Append("SELECT * FROM " + username + ".hit_DeleteProfileFromPos AS hdpfp");
			try
			{
				if (connection.State == ConnectionState.Open)
				{
					connection.Close();
				}

				using (connection)
				{
					connection.ConnectionString = connectString;
					connection.Open();
					SqlCommand comm = new SqlCommand(command.ToString(), connection);
					using (SqlDataReader reader = comm.ExecuteReader())
					{
						while (reader.Read())
						{
							custIds.Add(reader.GetInt64(0));
						}
					}
					comm.ExecuteNonQuery();
				}
				return custIds;
			}
			catch (Exception ex1)
			{
				logger.Error(ex1.ToString());
				var b = ex1.Message;
				return custIds;
			}
		}

		/// <summary>
		/// Insert points into hit_loyalty_actions
		/// </summary>
		/// <param name="username"></param>
		/// <param name="input"></param>
		/// <returns></returns>
		public bool DeleteCustomersProfile(string username, string input)
		{
			StringBuilder command = new StringBuilder();
			command.Clear();
			command.Append("DELETE FROM " + username + ".hit_DeleteProfileFromPos WHERE CustomerID IN (" + input + ")");

			return execSQLForDeleteCustomers(command.ToString());
		}


		/// <summary>
		/// Checks if reservation is checked in for PMS Protel
		/// </summary>
		/// <param name="regNo"></param>
		/// <returns></returns>
		public bool checkForPaymentChange(string regNo, string hotelType)
		{
			string query = "";
			switch (hotelType)
			{
				case "PROTEL":
					query = string.Format("SELECT CASE WHEN b.buchstatus = 1 THEN 'true' ELSE 'false' END checkIn FROM buch AS b WHERE b.leistacc = {0}", regNo);
					break;
				case "ERMIS":
					query = string.Format("SELECT CASE WHEN ISNULL( br.bereflag, 1) = 1 THEN 'true' ELSE 'false' END checkIn  \n"
										+ "FROM ErmisUser.bereg AS br  \n"
										+ "WHERE br.bereaaa = {0} ", regNo);
					break;
			}

			SqlDataReader da = returnResult(query);
			da.Read();
			string res = (string)da["checkIn"];
			da.Close();
			closeConnection();
			if (res == "true")
				return true;
			else
				return false;
		}

		//////////////////////////////////////// PROTEL REGION ////////////////////////////////////////
		/// <summary>
		/// 
		/// </summary>
		/// <param name="username"> afm of customer </param>
		/// <returns></returns>
		private string ProtelCustomersByAFM(string username)
		{
			string sql = "DECLARE @sCommand VARCHAR(20), @SQL VARCHAR(1000) \n"
						+ "IF OBJECT_ID('" + username + ".GetProfileByAFM') IS NULL \n"
						+ "	SET @sCommand = 'CREATE ' \n"
						+ "ELSE \n"
						+ "	SET @sCommand = 'ALTER ' \n"
						+ " \n"
						+ "SET @SQL = @sCommand+' PROCEDURE GetProfileByAFM @Afm VARCHAR(100) AS  \n"
						+ "		BEGIN  \n"
						+ "			SELECT ISNULL(k.kdnr,0) id, ISNULL(k.name1,'''') lastName, ISNULL(k.vorname,'''') eokName, ISNULL(k.afmno,'''') afmno, ISNULL(k.vatno,'''') doy, ISNULL(k.strasse,'''') addressName, ISNULL(k.strasse2,'''') address2, \n"
						+ "				ISNULL(k.ort,'''') city, ISNULL(k.plz,'''') postCode, ISNULL(n.codenr,0) natId, ISNULL(n.land,'''') nationality, CAST(ISNULL(k.landkz,0) AS VARCHAR(20)) countryId, ISNULL(nt.land,'''') country, ISNULL(k.telefonnr,'''') telefonnr, \n"
						+ "				ISNULL(k.funktel,'''') mobile, ISNULL(k.name2,'''') companyName, ISNULL(k.abteil,'''') job, ISNULL(k.vip,0) vip, '''' code \n"
						+ "			FROM kunden AS k \n"
						+ "			LEFT OUTER JOIN natcode AS n ON n.codenr = k.nat \n"
						+ "			LEFT OUTER JOIN natcode AS nt ON nt.codenr = k.landkz \n"
						+ "			WHERE ISNULL(k.afmno,'''') <> '''' AND k.afmno = @Afm \n"
						+ "		END' \n"
						+ "EXEC(@SQL)";
			return sql;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private string ProtelMethodsOfPayment()
		{
			string sql = "DECLARE @sCommand VARCHAR(20), @SQL VARCHAR(1000) \n"
						+ "IF NOT EXISTS (SELECT 1 FROM sys.procedures AS p WHERE p.name = 'GetMethodOfPayments') \n"
						+ "	SET @sCommand = 'CREATE ' \n"
						+ "ELSE \n"
						+ "	SET @sCommand = 'ALTER ' \n"
						+ "SET @SQL = @sCommand + 'PROCEDURE GetMethodOfPayments  AS  \n"
						+ "	BEGIN  \n"
						+ "		SELECT z.zanr Id, z.bez Descr \n"
						+ "		FROM zahlart AS z \n"
						+ "	END' \n"
						+ "EXEC(@SQL) \n"
						+ "";
			return sql;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private string ProtelInvoiceTypes()
		{
			string sql = "DECLARE @sCommand VARCHAR(20), @SQL VARCHAR(1000) \n"
						+ "IF NOT EXISTS (SELECT 1 FROM sys.procedures AS p WHERE p.name = 'GetInvoiceTypes') \n"
						+ "	SET @sCommand = 'CREATE ' \n"
						+ "ELSE \n"
						+ "	SET @sCommand = 'ALTER ' \n"
						+ "SET @SQL = @sCommand + 'PROCEDURE GetInvoiceTypes @mpehotel INT AS  \n"
						+ "	BEGIN \n"
						+ "		SELECT f.ref Id, f.[text] Descr, f.code Abrev \n"
						+ "		FROM fiscalcd AS f \n"
						+ "		WHERE f.mpehotel = @mpehotel	 \n"
						+ "	END' \n"
						+ "EXEC(@SQL)";
			return sql;
		}

		//////////////////////////////////////// ERMIS REGION ////////////////////////////////////////
		/// <summary>
		/// 
		/// </summary>
		/// <param name="username"> afm of customer </param>
		/// <returns></returns>
		private string ErmisCustomersByAFM(string username)
		{
			string sql = "DECLARE @sCommand VARCHAR(20), @SQL VARCHAR(1000) \n"
						+ "IF OBJECT_ID('Ermisuser.GetProfileByAFM') IS NULL \n"
						+ "	SET @sCommand = 'CREATE ' \n"
						+ "ELSE \n"
						+ "	SET @sCommand = 'ALTER ' \n"
						+ " \n"
						+ "SET @SQL = @sCommand+' PROCEDURE Ermisuser.GetProfileByAFM @Afm VARCHAR(100) AS \n"
						+ "BEGIN   \n"
						+ "	SELECT ROW_NUMBER() OVER (ORDER BY b.bepracode) id, ISNULL(b.bepraname,'''') lastName,ISNULL(b.bepraname2,'''') eokName,  b.bepraafm afmno, ISNULL(b.bepradoy,'''') doy,  \n"
						+ "		ISNULL(b.bepraadr,'''') addressName, '''' address2, ISNULL(b.bepracity,'''') city, ISNULL(b.beprapost,'''') postCode, -1 natId, '''' nationality,  \n"
						+ "		ISNULL(b.bepraeth,'''') countryId, ISNULL(be.beetdesc,'''') country,  ISNULL(b.bepraphon,'''') telefonnr, '''' mobile, ISNULL(bj.bejobdes,'''') job,  \n"
						+ "		0 vip, b.bepracode code \n"
						+ "	FROM ErmisUser.bepra AS b \n"
						+ "	LEFT OUTER JOIN ErmisUser.beeth AS be ON be.beetcode = b.bepraeth \n"
						+ "	LEFT OUTER JOIN ErmisUser.bejob AS bj ON bj.bejobkey = b.beprajob \n"
						+ "	WHERE ISNULL(b.bepraafm,'''') <> '''' AND ISNULL(b.bepraafm,'''') = @Afm \n"
						+ "END' \n"
						+ "EXEC(@SQL)";
			return sql;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private string ErmisMethodsOfPayment()
		{
			string sql = "DECLARE @sCommand VARCHAR(20), @SQL VARCHAR(1000) \n"
						+ "IF NOT EXISTS (SELECT 1 FROM sys.procedures AS p WHERE p.name = 'GetMethodOfPayments') \n"
						+ "	SET @sCommand = 'CREATE ' \n"
						+ "ELSE \n"
						+ "	SET @sCommand = 'ALTER ' \n"
						+ "SET @SQL = @sCommand + 'PROCEDURE ErmisUser.GetMethodOfPayments  AS  \n"
						+ "	BEGIN  \n"
						+ "		SELECT p.ID id, p.[Description] Descr \n"
						+ "		FROM ErmisUser.Payments AS p \n"
						+ "	END' \n"
						+ "EXEC(@SQL) \n"
						+ "";
			return sql;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private string ErmisInvoiceTypes()
		{
			string sql = "DECLARE @sCommand VARCHAR(20), @SQL VARCHAR(1000) \n"
						+ "IF NOT EXISTS (SELECT 1 FROM sys.procedures AS p WHERE p.name = 'GetInvoiceTypes') \n"
						+ "	SET @sCommand = 'CREATE ' \n"
						+ "ELSE \n"
						+ "	SET @sCommand = 'ALTER ' \n"
						+ "SET @SQL = @sCommand + 'PROCEDURE ErmisUser.GetInvoiceTypes @mpehotel INT AS  \n"
						+ "	BEGIN  \n"
						+ "		SELECT i.ID Id, i.Descr, i.Sunt Abrev  \n"
						+ "		FROM ErmisUser.Invoices AS i \n"
						+ "		WHERE ISNULL(i.SwActive,-1) <> 0 \n"
						+ "	END' \n"
						+ "EXEC(@SQL)";
			return sql;
		}

	}
}
