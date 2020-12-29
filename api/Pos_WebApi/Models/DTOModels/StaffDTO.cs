using System;
using System.Collections.Generic;

namespace Pos_WebApi.Models.DTOModels
{
    public class StaffDTO : IDTOModel<Staff>
    {
        public StaffDTO()
        {
            StaffAuthorization = new HashSet<StaffAuthorizationDTO>();
            StaffPositions = new HashSet<StaffPositionDTO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ImageUri { get; set; }
        public string Password { get; set; }
        public string Identification { get; set; }
        public string FullName { get { return (this.FirstName ?? "") + " " + (this.LastName ?? ""); } }
        public Nullable<bool> IsDeleted { get; set; }
        public long? DaStoreId { get; set; }
        public string DaStoreDescription { get; set; }
        public Nullable<bool> isAdmin { get; set; }
        public Nullable<bool> LogInAfterOrder { get; set; }

        public ICollection<StaffAuthorizationDTO> StaffAuthorization { get; set; }
        public ICollection<StaffPositionDTO> StaffPositions { get; set; }



        public Staff ToModel()
        {
            var model = new Staff()
            {
                Code = this.Code,
                FirstName = this.FirstName,
                LastName = this.LastName,
                ImageUri = this.ImageUri,
                Password = this.Password,
                Identification = this.Identification,
                DASTORE = this.DaStoreId,
                isAdmin = this.isAdmin,
                LogInAfterOrder = this.LogInAfterOrder
            };
            foreach (var sa in StaffAuthorization)
            {
                model.StaffAuthorization.Add(sa.ToModel());
            }


            foreach (var sp in StaffPositions)
            {
                model.AssignedPositions.Add(sp.ToModel());
            }
            return model;
        }

        public Staff UpdateModel(Staff model)
        {
            model.Code = this.Code;
            model.FirstName = this.FirstName;
            model.LastName = this.LastName;
            model.ImageUri = this.ImageUri;
            model.Password = this.Password;
            model.Identification = this.Identification;
            model.DASTORE = this.DaStoreId;
            model.isAdmin = this.isAdmin;
            model.LogInAfterOrder = this.LogInAfterOrder;

            return model;
        }
    }

    public class StaffAuthorizationDTO : IDTOModel<StaffAuthorization>
    {
        public long Id { get; set; }
        public Nullable<long> AuthorizedGroupId { get; set; }
        public Nullable<long> StaffId { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public long? DaStoreId { get; set; }
        public string DaStoreDescription { get; set; }

        public StaffAuthorization ToModel()
        {
            var model = new StaffAuthorization()
            {
                AuthorizedGroupId = this.AuthorizedGroupId,
                StaffId = this.StaffId
            };
            return model;
        }

        public StaffAuthorization UpdateModel(StaffAuthorization model)
        {
            model.AuthorizedGroupId = this.AuthorizedGroupId;
            model.StaffId = this.StaffId;
            return model;
        }
    }


    public class StaffPositionDTO : IDTOModel<AssignedPositions>
    {
        public long Id { get; set; }
        public Nullable<long> StaffPositionId { get; set; }
        public Nullable<long> StaffId { get; set; }
        public bool IsDeleted { get; set; }


        public string Description { get; set; }

        public AssignedPositions ToModel()
        {
            var model = new AssignedPositions()
            {
                StaffId = this.StaffId,
                StaffPositionId = this.StaffPositionId
            };
            return model;
        }

        public AssignedPositions UpdateModel(AssignedPositions model)
        {
            model.StaffId = this.StaffId;
            model.StaffPositionId = this.StaffPositionId;
            return model;
        }
    }
}
