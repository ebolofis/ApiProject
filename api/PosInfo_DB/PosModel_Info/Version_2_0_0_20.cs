﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_DTOs.PosModel_Info
{
    [DisplayName("Ver : 2.0.0.20")]
    public class Version_2_0_0_20
    {
        public List<string> Ver_2_0_0_20 { get; }

        public Version_2_0_0_20()
        {
            Ver_2_0_0_20 = new List<string>();
            Ver_2_0_0_20.Add("ALTER PROCEDURE [dbo].[TR_GetAvailableRestaurantsDatesAndTime] @TotProfiles VARCHAR(2000),  @TotRooms VARCHAR(2000), \n"
                           + "	@Paxes INT, @dur INT, @sLang VARCHAR(10), @RestId INT, @ActiveDate DATETIME AS  \n"
                           + "BEGIN  \n"
                           + "	/******************************************* \n"
                           + "	 * @TotProfiles		Total Profiles seperated by , ex(23,567,443452,33435) \n"
                           + "	 * @TotRooms		Total Rooms seperated by , ex(102,302,105,627) \n"
                           + "	 * @Paxes			Total Paxes for Reservation \n"
                           + "	 * @dur				Days to show results. Start from SYSTEM DATE (GETDATE()) \n"
                           + "	 * @sLang			Language to get data (GR,En,Ru,Fr,De) \n"
                           + "	 * @RestId			If @RestId id less than 0 then returns Available days per restaurant and type else \n"
                           + "	 *					If @RestId equals 0(zero) then For Current Date Max time per RestId else \n"
                           + "	 *						returns available times for specific restaurant and date \n"
                           + "	 * @ActiveDate		Specific date to get times for specific restaurant \n"
                           + "	 *  \n"
                           + "	 * Results \n"
                           + "	 *		For Available Restaurant the results are \n"
                           + "	 *			AvailDate		DateTime (Available Date),		 \n"
                           + "	 *			RestId			BigInt (Restaurant Id),	 \n"
                           + "	 *			RestaurantName	NVARCHAR(200) (Restaurant Name for selected Language),  \n"
                           + "	 *			Type			INT (meal type 0=> All, 1=> Lunch, 2=> Dinner) \n"
                           + "	 *		For Available Times Per Restaurant and Date the resylts are \n"
                           + "	 *			[Time]			Time (Available Time) \n"
                           + "	 *			[Type]			INT  (meal type 0=> All, 1=> Lunch, 2=> Dinner) \n"
                           + "	 *			 \n"
                           + "	 *******************************************/ \n"
                           + " \n"
                           + "	DECLARE @stDate DATETIME, @ProfileId INT, @tmpStr VARCHAR(100), @idx INT, @tRec INT,  @stTime TIME  \n"
                           + " \n"
                           + "	SELECT  @stDate = CAST(CONVERT(VARCHAR(10),GETDATE(),120) AS DATETIME) \n"
                           + "	SELECT @stTime = CAST(GETDATE() AS TIME) \n"
                           + "	   \n"
                           + "	/*Gets all profiles to a list*/ \n"
                           + "	DECLARE @Profiles TABLE (Id INT IDENTITY(1,1), ProfileId INT) \n"
                           + "	DECLARE @Rooms TABLE (Id INT IDENTITY(1,1), RoomNo VARCHAR(20)) \n"
                           + "	 \n"
                           + "	DECLARE @ReservationHist TABLE (RestrictId INT, Occupancy INT, WeekNo INT, RestId INT, ActiveDate DATETIME, roomNo VARCHAR(10), Meal_Type INT) \n"
                           + " \n"
                           + "	/*Temporary tables for @RestId < 1 to get union data*/ \n"
                           + "	DECLARE @tmpDates_1 TABLE (AvailDate DATETIME, RestId INT, RestaurantName NVARCHAR(200), [Type] INT) \n"
                           + "	DECLARE @Dates_Results TABLE (AvailDate DATETIME, RestId INT, RestaurantName NVARCHAR(200), [Type] INT) \n"
                           + " \n"
                           + "	/*Temporary tables for @RestId > 0 to get union data*/ \n"
                           + "	DECLARE @tmpTimes_1 TABLE ([Time] TIME, CapacityId INT, [Type] INT) \n"
                           + "	DECLARE @Times_Results TABLE ([Time] TIME, CapacityId INT, [Type] INT) \n"
                           + " \n"
                           + "	/*Temporary tables for @RestId > 0 to get union data*/ \n"
                           + "	DECLARE @tmpTimes_Rest0 TABLE (RestId INT, [Time] TIME, CapacityId INT, [Type] INT) \n"
                           + "	DECLARE @Times_Results_Rest0 TABLE (RestId INT, [Time] TIME, CapacityId INT, [Type] INT) \n"
                           + " \n"
                           + " \n"
                           + "	WHILE LEN(@TotProfiles) > 0 \n"
                           + "	BEGIN \n"
                           + "		IF(PATINDEX('%,%',@TotProfiles) > 0) \n"
                           + "		BEGIN \n"
                           + "			SET @tmpStr = SUBSTRING(@TotProfiles,0,PATINDEX('%,%',@TotProfiles)) \n"
                           + "			IF (ISNUMERIC(@tmpStr) = 1) \n"
                           + "				INSERT INTO @Profiles (ProfileId) VALUES(CAST(@tmpStr AS INT)) \n"
                           + "			SET @TotProfiles = SUBSTRING(@TotProfiles,LEN(@tmpStr+',')+1,LEN(@TotProfiles)) \n"
                           + "		END \n"
                           + "		ELSE \n"
                           + "		BEGIN \n"
                           + "			IF (ISNUMERIC(@TotProfiles) = 1) \n"
                           + "				INSERT INTO @Profiles (ProfileId) VALUES(CAST(@TotProfiles AS INT)) \n"
                           + "			SET @TotProfiles = NULL \n"
                           + "		END \n"
                           + "	END \n"
                           + " \n"
                           + "	WHILE LEN(@TotRooms) > 0 \n"
                           + "	BEGIN \n"
                           + "		IF(PATINDEX('%,%',@TotRooms) > 0) \n"
                           + "		BEGIN \n"
                           + "			SET @tmpStr = SUBSTRING(@TotRooms,0,PATINDEX('%,%',@TotRooms)) \n"
                           + "			IF (LTRIM(RTRIM(@tmpStr)) <> '') \n"
                           + "				INSERT INTO @Rooms(Roomno) VALUES(@tmpStr) \n"
                           + "			SET @TotRooms = SUBSTRING(@TotRooms,LEN(@tmpStr+',')+1,LEN(@TotRooms)) \n"
                           + "		END \n"
                           + "		ELSE \n"
                           + "		BEGIN \n"
                           + "			IF (LTRIM(RTRIM(@TotRooms)) <> '') \n"
                           + "				INSERT INTO @Rooms(Roomno) VALUES(@TotRooms) \n"
                           + "			SET @TotRooms = NULL \n"
                           + "		END \n"
                           + "	END \n"
                           + " \n"
                           + " \n"
                           + "	/*Makes period to rows. Period starts from @stDate date and end on @dur(number of days)*/ \n"
                           + "	DECLARE @Dates TABLE (dtCur DATETIME) \n"
                           + "	;WITH CTE_Range (Date) AS \n"
                           + "	( \n"
                           + "	  SELECT DATEADD(DAY,1,@stDate) \n"
                           + "	  UNION ALL \n"
                           + "	  SELECT DATEADD(DAY,1,Date) \n"
                           + "	  FROM CTE_Range \n"
                           + "	  WHERE Date < DATEADD(DAY,1,@stDate+@dur) \n"
                           + "	) \n"
                           + "	INSERT INTO @Dates(dtCur) \n"
                           + "	SELECT DATEADD(DAY,-1*1,Date) AS Date \n"
                           + "	FROM CTE_Range \n"
                           + "	WHERE Date < DATEADD(DAY,1*1, \n"
                           + "				 DATEADD(DAY,1*1,@stDate+@dur)) \n"
                           + "	ORDER BY Date \n"
                           + " \n"
                           + "	SELECT @idx = MIN(ID), @tRec = MAX(ID) FROM @Profiles \n"
                           + " \n"
                           + "	WHILE @idx <= @tRec \n"
                           + "	BEGIN \n"
                           + "		SELECT @ProfileId = ProfileId \n"
                           + "		FROM @Profiles  \n"
                           + "		WHERE Id = @idx \n"
                           + "	 \n"
                           + "		DELETE FROM @ReservationHist \n"
                           + "		/*Historic Table for Current Profile (gets number of reservations per room, Reservations per week and more) */ \n"
                           + "		INSERT INTO @ReservationHist (RestrictId, Occupancy, WeekNo, RestId, ActiveDate, roomNo, Meal_Type) \n"
                           + "		SELECT 1 RestrictId, COUNT(trc.ProtelId) Occupancy, DATEPART(wk, tr.ReservationDate) WeekNo, tr.RestId RestId, NULL ActiveDate, '' roomNo, 0 Meal_Type \n"
                           + "		FROM TR_ReservationCustomers AS trc  \n"
                           + "		INNER JOIN TR_Reservations AS tr ON tr.Id = trc.ReservationId \n"
                           + "		WHERE trc.ProtelId = @ProfileId \n"
                           + "		GROUP BY DATEPART(wk, tr.ReservationDate), tr.RestId \n"
                           + "		UNION ALL \n"
                           + "		SELECT 2 RestrictId, @Paxes Occupancy, 0 WeekNo, trra.RestId RestId, NULL ActiveDate, '' roomNo, 0 Meal_Type \n"
                           + "		FROM TR_Restrictions_Restaurants_Assoc AS trra  \n"
                           + "		UNION ALL \n"
                           + "		SELECT 3 RestrictId, COUNT(trc.ProtelId) Occupancy, DATEPART(wk, tr.ReservationDate) WeekNo, tr.RestId RestId, NULL ActiveDate, '' roomNo, 2 Meal_Type \n"
                           + "		FROM TR_ReservationCustomers AS trc \n"
                           + "		INNER JOIN TR_Reservations AS tr ON tr.Id = trc.ReservationId \n"
                           + "		INNER JOIN TR_Capacities AS tc ON tc.Id = tr.CapacityId AND tc.[Type] = 2 \n"
                           + "		WHERE trc.ProtelId = @ProfileId \n"
                           + "		GROUP BY tr.RestId, DATEPART(wk, tr.ReservationDate) \n"
                           + "		UNION ALL \n"
                           + "		SELECT 4 RestrictId, COUNT(trc.ProtelId) Occupancy, DATEPART(wk, tr.ReservationDate) WeekNo, tr.RestId RestId, NULL ActiveDate, '' roomNo, 2 Meal_Type \n"
                           + "		FROM TR_ReservationCustomers AS trc \n"
                           + "		INNER JOIN TR_Reservations AS tr ON tr.Id = trc.ReservationId \n"
                           + "		INNER JOIN TR_Capacities AS tc ON tc.Id = tr.CapacityId AND tc.[Type] = 1 \n"
                           + "		WHERE trc.ProtelId = @ProfileId \n"
                           + "		GROUP BY tr.RestId, DATEPART(wk, tr.ReservationDate) \n"
                           + "		UNION ALL \n"
                           + "		SELECT 5 RestrictId, COUNT(trc.RoomNum) Occupancy, 0 WeekNo, -1 RestId, CAST(tr.CreateDate AS DATE) ActiveDate, trc.RoomNum, 0 Meal_Type \n"
                           + "		FROM TR_ReservationCustomers AS trc \n"
                           + "		INNER JOIN TR_Reservations AS tr ON tr.Id = trc.ReservationId \n"
                           + "		INNER JOIN @Rooms r ON trc.RoomNum = r.RoomNo \n"
                           + "		WHERE trc.ProtelId = @ProfileId  \n"
                           + "		GROUP BY  trc.RoomNum, CAST(tr.CreateDate AS DATE) \n"
                           + "		UNION ALL \n"
                           + "		SELECT 6 RestrictId, SUM(tr.Couver) Occupancy,0 WeekNo, tr.RestId,tr.ReservationDate,'' roomNo, tr.CapacityId Meal_Type_Paxes \n"
                           + "		FROM TR_Reservations AS tr \n"
                           + "		INNER JOIN TR_Capacities AS tc ON tc.RestId = tr.RestId AND tc.Id = tr.CapacityId \n"
                           + "		WHERE tr.ReservationDate BETWEEN @stDate AND @stDate + @dur \n"
                           + "		GROUP BY tr.RestId, tr.CapacityId, tr.ReservationDate \n"
                           + " \n"
                           + "		IF (@RestId = 0) \n"
                           + "		BEGIN \n"
                           + "			SET @ActiveDate = CAST(GETDATE() AS DATE) \n"
                           + "			DELETE FROM @tmpTimes_Rest0 \n"
                           + "			INSERT INTO @tmpTimes_Rest0 (RestId, [Time],CapacityId, [Type]) \n"
                           + "			/*Selects all data for current reservation */ \n"
                           + "			SELECT DISTINCT tr.Id,	tc.[Time], tc.Id, tc.[Type] \n"
                           + "			FROM TR_Restaurants AS tr \n"
                           + "			INNER JOIN TR_Capacities AS tc ON tc.RestId = tr.Id \n"
                           + "			LEFT OUTER JOIN TR_ReservationCustomers AS trc ON trc.ProtelId = @ProfileId \n"
                           + "			OUTER APPLY ( \n"
                           + "				SELECT SUM(trRes.couver) couver \n"
                           + "				FROM TR_Reservations AS trRes  \n"
                           + "				WHERE trRes.RestId = tr.Id AND trRes.CapacityId = tc.Id AND  \n"
                           + "					trRes.ReservationDate BETWEEN @stDate AND @stDate + @dur \n"
                           + "			) trr \n"
                           + "			INNER JOIN @Dates d ON 1=1 AND d.dtCur = @ActiveDate \n"
                           + "			LEFT OUTER JOIN TR_ExcludeDays AS ted ON ted.RestId = tr.Id AND ted.[Type] = tc.[Type] AND ted.[Date] = d.dtCur \n"
                           + "			LEFT OUTER JOIN TR_OvewrittenCapacities AS toc ON toc.RestId = tr.Id AND toc.CapacityId = tc.Id AND toc.[Date] = d.dtCur \n"
                           + "			OUTER APPLY ( \n"
                           + "				SELECT r.RestrictId, trra.N, r.Occupancy \n"
                           + "				FROM @ReservationHist r  \n"
                           + "				INNER JOIN TR_Restrictions_Restaurants_Assoc AS trra ON trra.RestrictId = r.RestrictId AND trra.RestId = tr.Id \n"
                           + "				WHERE r.RestrictId = 1 AND DATEPART(wk,d.dtCur) = r.WeekNo AND r.RestId = tr.Id \n"
                           + "			) r1 \n"
                           + "			OUTER APPLY ( \n"
                           + "				SELECT trra.N Restrict2_Occupancy  \n"
                           + "				FROM @ReservationHist r  \n"
                           + "				INNER JOIN TR_Restrictions_Restaurants_Assoc AS trra ON trra.RestrictId = r.RestrictId AND trra.RestId = tr.Id \n"
                           + "				WHERE r.RestrictId = 2 \n"
                           + "			) r2 \n"
                           + "			OUTER APPLY ( \n"
                           + "				SELECT r.RestrictId, trra.N, r.Occupancy \n"
                           + "				FROM @ReservationHist r  \n"
                           + "				INNER JOIN TR_Restrictions_Restaurants_Assoc AS trra ON trra.RestrictId = r.RestrictId AND trra.RestId = tr.Id \n"
                           + "				WHERE r.RestrictId = 3 AND DATEPART(wk,d.dtCur) = r.WeekNo AND r.Meal_Type = tc.[Type] \n"
                           + "			) r3 \n"
                           + "			OUTER APPLY ( \n"
                           + "				SELECT r.RestrictId, trra.N, r.Occupancy \n"
                           + "				FROM @ReservationHist r  \n"
                           + "				INNER JOIN TR_Restrictions_Restaurants_Assoc AS trra ON trra.RestrictId = r.RestrictId AND trra.RestId = tr.Id \n"
                           + "				WHERE r.RestrictId = 4 AND DATEPART(wk,d.dtCur) = r.WeekNo AND r.Meal_Type = tc.[Type] \n"
                           + "			) r4 \n"
                           + "			OUTER APPLY ( \n"
                           + "				SELECT r.RestrictId, trra.N, r.Occupancy \n"
                           + "				FROM @ReservationHist r  \n"
                           + "				INNER JOIN TR_Restrictions_Restaurants_Assoc AS trra ON trra.RestrictId = r.RestrictId  \n"
                           + "				WHERE r.RestrictId = 5 AND r.ActiveDate = CAST(GETDATE() AS DATE) AND r.roomNo = trc.RoomNum \n"
                           + "			) r5 \n"
                           + "			OUTER APPLY( \n"
                           + "				SELECT ISNULL(r.Occupancy,0) Occupancy \n"
                           + "				FROM @ReservationHist r \n"
                           + "				WHERE r.RestrictId = 6 AND r.ActiveDate = d.dtCur AND r.RestId = tr.Id AND r.Meal_Type = tc.Id \n"
                           + "			) r6 \n"
                           + "			WHERE  \n"
                           + "				ted.[Date] IS NULL AND ted.[Type] IS NULL AND  \n"
                           + "				CASE WHEN toc.Id IS NOT NULL THEN ISNULL(toc.Capacity,0) - (ISNULL(@Paxes,0)+ISNULL(r6.Occupancy,0))  \n"
                           + "				ELSE ISNULL(tc.Capacity,0) - (ISNULL(@Paxes,0)+ISNULL(r6.Occupancy,0)) END >= 0 \n"
                           + "				AND (ISNULL(r1.N,0) > ISNULL(r1.Occupancy,0) OR r1.RestrictId IS NULL)  \n"
                           + "				AND ISNULL(r2.Restrict2_Occupancy,1000) > @Paxes  \n"
                           + "				AND r3.RestrictId IS NULL \n"
                           + "				AND r4.RestrictId IS NULL \n"
                           + "				AND (ISNULL(r5.N,0) > ISNULL(r5.Occupancy,0) OR r5.RestrictId IS NULL) \n"
                           + "			ORDER BY tc.[Time], tc.[Type]	 \n"
                           + "		 \n"
                           + "		 \n"
                           + "			IF (@idx = 1)  \n"
                           + "				INSERT INTO @Times_Results_Rest0 (RestId, [Time],CapacityId, [Type]) \n"
                           + "				SELECT RestId, [Time], CapacityId, [Type] \n"
                           + "				FROM @tmpTimes_Rest0 \n"
                           + "			ELSE \n"
                           + "			BEGIN  \n"
                           + "				DELETE d \n"
                           + "				FROM @Times_Results_Rest0 d \n"
                           + "				LEFT OUTER JOIN @tmpTimes_Rest0 t ON t.[Time] = d.[Time] AND t.[Type] = d.[Type] AND t.CapacityId = d.CapacityId AND d.RestId = t.RestId \n"
                           + "				WHERE t.[Time] IS NULL \n"
                           + "			 \n"
                           + "				INSERT INTO @Times_Results_Rest0 (RestId, [Time], CapacityId, [Type]) \n"
                           + "				SELECT RestId, [Time], CapacityId, [Type] \n"
                           + "				FROM @tmpTimes_Rest0 \n"
                           + "			END \n"
                           + "			 \n"
                           + "		END	 \n"
                           + "		ELSE IF (@RestId < 0) \n"
                           + "		BEGIN  \n"
                           + "			DELETE FROM @tmpDates_1 \n"
                           + "			INSERT INTO @tmpDates_1 (AvailDate, RestId, RestaurantName, [Type]) \n"
                           + "			/*Selects all data for current reservation */ \n"
                           + "			SELECT DISTINCT	d.dtCur AvailDate, tr.Id,  \n"
                           + "				CASE WHEN UPPER(@sLang) = 'GR' THEN tr.NameGR \n"
                           + "					 WHEN UPPER(@sLang) = 'EN' THEN tr.NameEn \n"
                           + "					 WHEN UPPER(@sLang) = 'RU' THEN tr.NameRu \n"
                           + "					 WHEN UPPER(@sLang) = 'FR' THEN tr.NameFr \n"
                           + "					 WHEN UPPER(@sLang) = 'DE' THEN tr.NameDe \n"
                           + "				ELSE tr.NameGR END ResturantName, tc.[Type] \n"
                           + "			FROM TR_Restaurants AS tr \n"
                           + "			INNER JOIN TR_Capacities AS tc ON tc.RestId = tr.Id \n"
                           + "			LEFT OUTER JOIN TR_ReservationCustomers AS trc ON trc.ProtelId = @ProfileId \n"
                           + "			 \n"
                           + "			OUTER APPLY ( \n"
                           + "				SELECT SUM(trRes.couver) couver \n"
                           + "				FROM TR_Reservations AS trRes  \n"
                           + "				WHERE trRes.RestId = tr.Id AND trRes.CapacityId = tc.Id AND  \n"
                           + "					trRes.ReservationDate BETWEEN @stDate AND @stDate + @dur \n"
                           + "			) trr \n"
                           + "			INNER JOIN @Dates d ON 1=1  \n"
                           + "			LEFT OUTER JOIN TR_ExcludeDays AS ted ON ted.RestId = tr.Id AND ted.[Type] = tc.[Type] AND ted.[Date] = d.dtCur \n"
                           + "			LEFT OUTER JOIN TR_OvewrittenCapacities AS toc ON toc.RestId = tr.Id AND toc.CapacityId = tc.Id AND toc.[Date] = d.dtCur \n"
                           + "			OUTER APPLY ( \n"
                           + "				SELECT r.RestrictId, trra.N, r.Occupancy \n"
                           + "				FROM @ReservationHist r  \n"
                           + "				INNER JOIN TR_Restrictions_Restaurants_Assoc AS trra ON trra.RestrictId = r.RestrictId AND trra.RestId = tr.Id \n"
                           + "				WHERE r.RestrictId = 1 AND DATEPART(wk,d.dtCur) = r.WeekNo AND r.RestId = tr.Id  \n"
                           + "			) r1 \n"
                           + "			OUTER APPLY ( \n"
                           + "				SELECT trra.N Restrict2_Occupancy  \n"
                           + "				FROM @ReservationHist r  \n"
                           + "				INNER JOIN TR_Restrictions_Restaurants_Assoc AS trra ON trra.RestrictId = r.RestrictId AND trra.RestId = tr.Id \n"
                           + "				WHERE r.RestrictId = 2 \n"
                           + "			) r2 \n"
                           + "			OUTER APPLY ( \n"
                           + "				SELECT r.RestrictId, trra.N, r.Occupancy \n"
                           + "				FROM @ReservationHist r  \n"
                           + "				INNER JOIN TR_Restrictions_Restaurants_Assoc AS trra ON trra.RestrictId = r.RestrictId AND trra.RestId = tr.Id \n"
                           + "				WHERE r.RestrictId = 3 AND DATEPART(wk,d.dtCur) = r.WeekNo AND r.Meal_Type = tc.[Type] \n"
                           + "			) r3 \n"
                           + "			OUTER APPLY ( \n"
                           + "				SELECT r.RestrictId, trra.N, r.Occupancy  \n"
                           + "				FROM @ReservationHist r  \n"
                           + "				INNER JOIN TR_Restrictions_Restaurants_Assoc AS trra ON trra.RestrictId = r.RestrictId AND trra.RestId = tr.Id \n"
                           + "				WHERE r.RestrictId = 4 AND DATEPART(wk,d.dtCur) = r.WeekNo AND r.Meal_Type = tc.[Type] \n"
                           + "			) r4 \n"
                           + "			OUTER APPLY ( \n"
                           + "				SELECT r.RestrictId, trra.N, r.Occupancy \n"
                           + "				FROM @ReservationHist r  \n"
                           + "				INNER JOIN TR_Restrictions_Restaurants_Assoc AS trra ON trra.RestrictId = r.RestrictId \n"
                           + "				WHERE r.RestrictId = 5 AND r.ActiveDate = CAST(GETDATE() AS DATE) AND r.roomNo = trc.RoomNum \n"
                           + "			) r5 \n"
                           + "			OUTER APPLY( \n"
                           + "				SELECT ISNULL(r.Occupancy,0) Occupancy \n"
                           + "				FROM @ReservationHist r \n"
                           + "				WHERE r.RestrictId = 6 AND r.ActiveDate = d.dtCur AND r.RestId = tr.Id AND r.Meal_Type = tc.Id \n"
                           + "			) r6 \n"
                           + "			WHERE  \n"
                           + "				ted.[Date] IS NULL AND ted.[Type] IS NULL AND  \n"
                           + "				CASE WHEN toc.Id IS NOT NULL THEN ISNULL(toc.Capacity,0) - (ISNULL(@Paxes,0)+ISNULL(r6.Occupancy,0))  \n"
                           + "				ELSE ISNULL(tc.Capacity,0) - (ISNULL(@Paxes,0)+ISNULL(r6.Occupancy,0)) END >= 0 \n"
                           + "				AND (ISNULL(r1.N,0) > ISNULL(r1.Occupancy,0) OR r1.RestrictId IS NULL)  \n"
                           + "				AND ISNULL(r2.Restrict2_Occupancy,1000) >= @Paxes  \n"
                           + "				AND r3.RestrictId IS NULL \n"
                           + "				AND r4.RestrictId IS NULL \n"
                           + "				AND (ISNULL(r5.N,0) > ISNULL(r5.Occupancy,0) OR r5.RestrictId IS NULL) \n"
                           + "			ORDER BY d.dtCur, ResturantName, tc.[Type] \n"
                           + " \n"
                           + "			IF (@idx = 1)  \n"
                           + "				INSERT INTO @Dates_Results (AvailDate, RestId, RestaurantName, [Type]) \n"
                           + "				SELECT AvailDate, RestId, RestaurantName, [Type] \n"
                           + "				FROM @tmpDates_1 \n"
                           + "			ELSE \n"
                           + "			BEGIN  \n"
                           + "				DELETE d \n"
                           + "				FROM @Dates_Results d \n"
                           + "				LEFT OUTER JOIN @tmpDates_1 t ON t.AvailDate = d.AvailDate AND t.RestId = d.RestId AND t.RestaurantName = d.RestaurantName AND t.[Type] = d.[Type] \n"
                           + "				WHERE t.AvailDate IS NULL \n"
                           + "			 \n"
                           + "				INSERT INTO @Dates_Results (AvailDate, RestId, RestaurantName, [Type]) \n"
                           + "				SELECT AvailDate, RestId, RestaurantName, [Type] \n"
                           + "				FROM @tmpDates_1 \n"
                           + "			END \n"
                           + "		END \n"
                           + "		ELSE \n"
                           + "		BEGIN  \n"
                           + "			DELETE FROM @tmpTimes_1 \n"
                           + "			INSERT INTO @tmpTimes_1 ([Time],CapacityId, [Type]) \n"
                           + "			/*Selects all data for current reservation */ \n"
                           + "			SELECT DISTINCT	tc.[Time], tc.Id, tc.[Type] \n"
                           + "			FROM TR_Restaurants AS tr \n"
                           + "			INNER JOIN TR_Capacities AS tc ON tc.RestId = tr.Id \n"
                           + "			LEFT OUTER JOIN TR_ReservationCustomers AS trc ON trc.ProtelId = @ProfileId \n"
                           + "			OUTER APPLY ( \n"
                           + "				SELECT SUM(trRes.couver) couver \n"
                           + "				FROM TR_Reservations AS trRes  \n"
                           + "				WHERE trRes.RestId = tr.Id AND trRes.CapacityId = tc.Id AND  \n"
                           + "					trRes.ReservationDate BETWEEN @stDate AND @stDate + @dur \n"
                           + "			) trr \n"
                           + "			INNER JOIN @Dates d ON 1=1 AND d.dtCur = @ActiveDate \n"
                           + "			LEFT OUTER JOIN TR_ExcludeDays AS ted ON ted.RestId = tr.Id AND ted.[Type] = tc.[Type] AND ted.[Date] = d.dtCur \n"
                           + "			LEFT OUTER JOIN TR_OvewrittenCapacities AS toc ON toc.RestId = tr.Id AND toc.CapacityId = tc.Id AND toc.[Date] = d.dtCur \n"
                           + "			OUTER APPLY ( \n"
                           + "				SELECT r.RestrictId, trra.N, r.Occupancy \n"
                           + "				FROM @ReservationHist r  \n"
                           + "				INNER JOIN TR_Restrictions_Restaurants_Assoc AS trra ON trra.RestrictId = r.RestrictId AND trra.RestId = tr.Id \n"
                           + "				WHERE r.RestrictId = 1 AND DATEPART(wk,d.dtCur) = r.WeekNo  AND r.RestId = tr.Id \n"
                           + "			) r1 \n"
                           + "			OUTER APPLY ( \n"
                           + "				SELECT trra.N Restrict2_Occupancy  \n"
                           + "				FROM @ReservationHist r  \n"
                           + "				INNER JOIN TR_Restrictions_Restaurants_Assoc AS trra ON trra.RestrictId = r.RestrictId AND trra.RestId = tr.Id \n"
                           + "				WHERE r.RestrictId = 2 \n"
                           + "			) r2 \n"
                           + "			OUTER APPLY ( \n"
                           + "				SELECT r.RestrictId, trra.N, r.Occupancy \n"
                           + "				FROM @ReservationHist r  \n"
                           + "				INNER JOIN TR_Restrictions_Restaurants_Assoc AS trra ON trra.RestrictId = r.RestrictId AND trra.RestId = tr.Id \n"
                           + "				WHERE r.RestrictId = 3 AND DATEPART(wk,d.dtCur) = r.WeekNo AND r.Meal_Type = tc.[Type] \n"
                           + "			) r3 \n"
                           + "			OUTER APPLY ( \n"
                           + "				SELECT r.RestrictId, trra.N, r.Occupancy \n"
                           + "				FROM @ReservationHist r  \n"
                           + "				INNER JOIN TR_Restrictions_Restaurants_Assoc AS trra ON trra.RestrictId = r.RestrictId AND trra.RestId = tr.Id \n"
                           + "				WHERE r.RestrictId = 4 AND DATEPART(wk,d.dtCur) = r.WeekNo AND r.Meal_Type = tc.[Type] \n"
                           + "			) r4 \n"
                           + "			OUTER APPLY ( \n"
                           + "				SELECT r.RestrictId, trra.N, r.Occupancy \n"
                           + "				FROM @ReservationHist r  \n"
                           + "				INNER JOIN TR_Restrictions_Restaurants_Assoc AS trra ON trra.RestrictId = r.RestrictId AND trra.RestId = tr.Id \n"
                           + "				WHERE r.RestrictId = 5 AND r.ActiveDate = CAST(GETDATE() AS DATE) AND r.roomNo = trc.RoomNum \n"
                           + "			) r5 \n"
                           + "			OUTER APPLY( \n"
                           + "				SELECT ISNULL(r.Occupancy,0) Occupancy \n"
                           + "				FROM @ReservationHist r \n"
                           + "				WHERE r.RestrictId = 6 AND r.ActiveDate = d.dtCur AND r.Meal_Type = tc.Id \n"
                           + "			) r6 \n"
                           + "			WHERE  \n"
                           + "				ted.[Date] IS NULL AND ted.[Type] IS NULL AND  \n"
                           + "				CASE WHEN toc.Id IS NOT NULL THEN ISNULL(toc.Capacity,0) - (ISNULL(@Paxes,0)+ISNULL(r6.Occupancy,0))  \n"
                           + "				ELSE ISNULL(tc.Capacity,0) - (ISNULL(@Paxes,0)+ISNULL(r6.Occupancy,0)) END >= 0 \n"
                           + "				AND (ISNULL(r1.N,0) > ISNULL(r1.Occupancy,0) OR r1.RestrictId IS NULL)  \n"
                           + "				AND ISNULL(r2.Restrict2_Occupancy,1000) > @Paxes  \n"
                           + "				AND r3.RestrictId IS NULL \n"
                           + "				AND r4.RestrictId IS NULL \n"
                           + "				AND (ISNULL(r5.N,0) > ISNULL(r5.Occupancy,0) OR r5.RestrictId IS NULL) \n"
                           + "				AND tr.Id = @RestId \n"
                           + "			ORDER BY tc.[Time], tc.[Type]	 \n"
                           + "		 \n"
                           + "			IF (@idx = 1)  \n"
                           + "				INSERT INTO @Times_Results ([Time],CapacityId, [Type]) \n"
                           + "				SELECT [Time], CapacityId, [Type] \n"
                           + "				FROM @tmpTimes_1 \n"
                           + "			ELSE \n"
                           + "			BEGIN  \n"
                           + "				DELETE d \n"
                           + "				FROM @Times_Results d \n"
                           + "				LEFT OUTER JOIN @tmpTimes_1 t ON t.[Time] = d.[Time] AND t.[Type] = d.[Type] AND t.CapacityId = d.CapacityId \n"
                           + "				WHERE t.[Time] IS NULL \n"
                           + "			 \n"
                           + "				INSERT INTO @Times_Results ([Time], CapacityId, [Type]) \n"
                           + "				SELECT [Time], CapacityId, [Type] \n"
                           + "				FROM @tmpTimes_1 \n"
                           + "			END \n"
                           + "		 \n"
                           + "		END	 \n"
                           + "		SET @idx = @idx + 1 \n"
                           + "	END \n"
                           + " \n"
                           + "	IF (@RestId < 0) \n"
                           + "		SELECT DISTINCT *  \n"
                           + "		FROM @Dates_Results  \n"
                           + "		ORDER BY AvailDate, RestId, [Type] \n"
                           + "	ELSE \n"
                           + "	IF (@RestId = 0) \n"
                           + "		SELECT RestId,MAX(TIME) TIME, [Type] \n"
                           + "		FROM @tmpTimes_Rest0 \n"
                           + "		GROUP BY RestId, [Type] \n"
                           + "	ELSE \n"
                           + "	BEGIN  \n"
                           + "		IF (@stDate = @ActiveDate) \n"
                           + "			SELECT DISTINCT CapacityId, [Time], [Type] \n"
                           + "			FROM @Times_Results  \n"
                           + "			WHERE [Time] > @stTime \n"
                           + "			ORDER BY [Time], [Type] \n"
                           + "		ELSE  \n"
                           + "			SELECT DISTINCT CapacityId, [Time], [Type] \n"
                           + "			FROM @Times_Results  \n"
                           + "			ORDER BY [Time], [Type] \n"
                           + "	END \n"
                           + "END");
        }
    }
}
