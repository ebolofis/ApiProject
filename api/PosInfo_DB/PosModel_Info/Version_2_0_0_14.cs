using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_DTOs.PosModel_Info
{
    [DisplayName("Ver : 2.0.0.14")]
    public class Version_2_0_0_14
    {
        public List<string> Ver_2_0_0_14 { get; }

        public Version_2_0_0_14()
        {
            Ver_2_0_0_14 = new List<string>();
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Id Key Record' ,  \n"
                           + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_Capacities',  \n"
                           + "	@level2type=N'COLUMN',@level2name=N'Id'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'TR_Restaurants.Id' ,  \n"
                           + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_Capacities',  \n"
                           + "	@level2type=N'COLUMN',@level2name=N'RestId'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'num of max customers' ,  \n"
                           + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_Capacities',  \n"
                           + "	@level2type=N'COLUMN',@level2name=N'Capacity'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'0:AllDay,1:Lunch,2:Dinner' ,  \n"
                           + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_Capacities',  \n"
                           + "	@level2type=N'COLUMN',@level2name=N'Type'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Time ex: 23:30' ,  \n"
                           + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_Capacities',  \n"
                           + "	@level2type=N'COLUMN',@level2name=N'Time'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Id Record key' ,  \n"
                           + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_ExcludeDays',  \n"
                           + "	@level2type=N'COLUMN',@level2name=N'Id'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'TR_Restaurants.Id' ,  \n"
                           + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_ExcludeDays',  \n"
                           + "	@level2type=N'COLUMN',@level2name=N'RestId'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Date of Unavailability' ,  \n"
                           + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_ExcludeDays',  \n"
                           + "	@level2type=N'COLUMN',@level2name=N'Date'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'0:AllDay,1:Lunch,2:Dinner' ,  \n"
                           + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_ExcludeDays',  \n"
                           + "	@level2type=N'COLUMN',@level2name=N'Type'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Id Record key' ,  \n"
                           + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_ExcludeRestrictions',  \n"
                           + "	@level2type=N'COLUMN',@level2name=N'Id'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'TR_Restaurants.Id' ,  \n"
                           + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_ExcludeRestrictions',  \n"
                           + "	@level2type=N'COLUMN',@level2name=N'RestId'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Id Record key' ,  \n"
                           + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_OvewrittenCapacities',  \n"
                           + "	@level2type=N'COLUMN',@level2name=N'Id'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'TR_Restaurants.Id' ,  \n"
                           + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_OvewrittenCapacities',  \n"
                           + "	@level2type=N'COLUMN',@level2name=N'RestId'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'TR_Capacities.Id' ,  \n"
                           + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_OvewrittenCapacities',  \n"
                           + "	@level2type=N'COLUMN',@level2name=N'CapacityId'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'num of max customers' ,  \n"
                           + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_OvewrittenCapacities',  \n"
                           + "	@level2type=N'COLUMN',@level2name=N'Capacity'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Date of overwrite' ,  \n"
                           + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_OvewrittenCapacities',  \n"
                           + "	@level2type=N'COLUMN',@level2name=N'Date'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Id Record Key' ,  \n"
                           + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_ReservationCustomers',  \n"
                           + "	@level2type=N'COLUMN',@level2name=N'Id'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Protel Profile Id' ,  \n"
                           + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_ReservationCustomers',  \n"
                           + "	@level2type=N'COLUMN',@level2name=N'ProtelId'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ProtelName (encrypted)' ,  \n"
                           + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_ReservationCustomers',  \n"
                           + "	@level2type=N'COLUMN',@level2name=N'ProtelName'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Name given by the customer (encrypted)' ,  \n"
                           + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_ReservationCustomers',  \n"
                           + "	@level2type=N'COLUMN',@level2name=N'ReservationName'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Room number' ,  \n"
                           + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_ReservationCustomers',  \n"
                           + "	@level2type=N'COLUMN',@level2name=N'RoomNum'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'email (encrypted)' ,  \n"
                           + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_ReservationCustomers',  \n"
                           + "	@level2type=N'COLUMN',@level2name=N'Email'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'TR_Reservations.Id' ,  \n"
                           + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_ReservationCustomers',  \n"
                           + "	@level2type=N'COLUMN',@level2name=N'ReservationId'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Id record key' ,  \n"
                           + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_Reservations',  \n"
                           + "	@level2type=N'COLUMN',@level2name=N'Id'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'TR_Restaurants.Id' ,  \n"
                           + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_Reservations',  \n"
                           + "	@level2type=N'COLUMN',@level2name=N'RestId'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'TR_Capacities.Id' ,  \n"
                                       + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_Reservations',  \n"
                                       + "	@level2type=N'COLUMN',@level2name=N'CapacityId'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Num of people' ,  \n"
                                       + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_Reservations',  \n"
                                       + "	@level2type=N'COLUMN',@level2name=N'Couver'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Reservation Date' ,  \n"
                                                   + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_Reservations',  \n"
                                                   + "	@level2type=N'COLUMN',@level2name=N'ReservationDate'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'TR_Capacities.Time' ,  \n"
                                                   + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_Reservations',  \n"
                                                   + "	@level2type=N'COLUMN',@level2name=N'ReservationTime'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'0: Active, 1: Cancel' ,  \n"
                                                   + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_Reservations',  \n"
                                                   + "	@level2type=N'COLUMN',@level2name=N'Status'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'HardCode Id' ,  \n"
                                                   + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_Restrictions',  \n"
                                                   + "	@level2type=N'COLUMN',@level2name=N'Id'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Id Record key' ,  \n"
                                                   + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_Restrictions_Restaurants_Assoc',  \n"
                                                   + "	@level2type=N'COLUMN',@level2name=N'Id'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'TR_Restrictions.Id' ,  \n"
                                                   + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_Restrictions_Restaurants_Assoc',  \n"
                                                   + "	@level2type=N'COLUMN',@level2name=N'RestrictId'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'TR_Restaurants.Id' ,  \n"
                                                   + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_Restrictions_Restaurants_Assoc',  \n"
                                                   + "	@level2type=N'COLUMN',@level2name=N'RestId'");
            Ver_2_0_0_14.Add("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Restriction number' ,  \n"
                                                   + "	@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TR_Restrictions_Restaurants_Assoc',  \n"
                                                   + "	@level2type=N'COLUMN',@level2name=N'N'");
            Ver_2_0_0_14.Add("CREATE UNIQUE NONCLUSTERED INDEX [PK_TR_Capacities_Unique] ON [dbo].[TR_Capacities] \n"
                            + "( \n"
                            + "	[RestId] ASC, \n"
                            + "	[Time] ASC \n"
                            + ")");
            Ver_2_0_0_14.Add("CREATE UNIQUE NONCLUSTERED INDEX [PK_TR_ExcludeDays_Unique] ON [dbo].[TR_ExcludeDays] \n"
                           + "( \n"
                           + "	[RestId] ASC, \n"
                           + "	[Date] ASC, \n"
                           + "	[Type] ASC \n"
                           + ")");
            Ver_2_0_0_14.Add("CREATE UNIQUE NONCLUSTERED INDEX [PK_TR_ExcludeRestrictions_Unique] ON [dbo].[TR_ExcludeRestrictions] \n"
                           + "( \n"
                           + "	[RestRestrictAssocId] ASC, \n"
                           + "	[Date] ASC \n"
                           + ")");
            Ver_2_0_0_14.Add("CREATE UNIQUE NONCLUSTERED INDEX [TR_ExcludeRestrictions_Unique2] ON [dbo].[TR_ExcludeRestrictions] \n"
                           + "( \n"
                           + "	[RestId] ASC, \n"
                           + "	[RestRestrictAssocId] ASC, \n"
                           + "	[Date] ASC \n"
                           + ")");
            Ver_2_0_0_14.Add("CREATE UNIQUE NONCLUSTERED INDEX [PK_TR_OvewrittenCapacities_Unique] ON [dbo].[TR_OvewrittenCapacities] \n"
                           + "( \n"
                           + "	[RestId] ASC, \n"
                           + "	[Date] ASC \n"
                           + ")");
            Ver_2_0_0_14.Add("CREATE UNIQUE NONCLUSTERED INDEX [PK_TR_OvewrittenCapacities_Unique2] ON [dbo].[TR_OvewrittenCapacities] \n"
                           + "( \n"
                           + "	[RestId] ASC, \n"
                           + "	[CapacityId] ASC \n"
                           + ")");
            Ver_2_0_0_14.Add("CREATE NONCLUSTERED INDEX [PK_TR_ReservationCustomers_ReservationId] ON [dbo].[TR_ReservationCustomers] \n"
                           + "( \n"
                           + "	[ReservationId] ASC \n"
                           + ")");
            Ver_2_0_0_14.Add("CREATE NONCLUSTERED INDEX [PK_TR_Reservations_index1] ON [dbo].[TR_Reservations] \n"
                           + "( \n"
                           + "	[RestId] ASC, \n"
                           + "	[ReservationDate] ASC, \n"
                           + "	[ReservationTime] ASC, \n"
                           + "	[Status] ASC \n"
                           + ")");
            Ver_2_0_0_14.Add("CREATE UNIQUE NONCLUSTERED INDEX [PK__Restrictions_Restaurants_Assoc_Unique] ON [dbo].[TR_Restrictions_Restaurants_Assoc] \n"
                           + "( \n"
                           + "	[RestId] ASC, \n"
                           + "	[RestrictId] ASC \n"
                           + ")");
        }
    }
}
