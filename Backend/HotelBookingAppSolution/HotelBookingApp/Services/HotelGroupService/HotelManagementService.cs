﻿using HotelBookingApp.DTO.HotelGroupDTO;
using HotelBookingApp.Exceptions;
using HotelBookingApp.Interface.IRepository.IHotels;
using HotelBookingApp.Interface.IRepository.IHotels.IHotelBranches;
using HotelBookingApp.Interface.IRepository.IHotels.IHotelGroups;
using HotelBookingApp.Interface.IService.IHotelGroupService;

namespace HotelBookingApp.Services.HotelGroupService
{
    public class HotelManagementService : IHotelManagementService
    {
        public readonly IHotelBranchRepository _hotelBranchRepository;
        public readonly IHotelGroupRepository _hotelGroupRepository;
        public readonly IHotelDemographicsRepository _hotelDemographicsRepository;
        public readonly IHotelImagesRepository _hotelImagesRepository;
        public readonly IHotelBranchRulesRepository _hotelBranchRulesRepository;
        public readonly IHotelAmenitiesRepository _hotelAmenitiesRepository;
        public readonly IRoomTypeRepository _roomTypeRepository;
        public readonly IRoomAmenitiesRepository _roomAmenitiesRepository;
        public HotelManagementService(IHotelBranchRepository hotelBranchRepository, IHotelDemographicsRepository hotelDemographicsRepository, IHotelImagesRepository hotelImagesRepository, IHotelBranchRulesRepository hotelBranchRulesRepository, IHotelAmenitiesRepository hotelAmenitiesRepository, IRoomAmenitiesRepository roomAmenitiesRepository, IRoomTypeRepository roomTypeRepository, IHotelGroupRepository hotelGroupRepository)
        {
            _hotelBranchRepository = hotelBranchRepository;
            _hotelDemographicsRepository = hotelDemographicsRepository;
            _hotelImagesRepository = hotelImagesRepository;
            _hotelBranchRulesRepository = hotelBranchRulesRepository;
            _hotelAmenitiesRepository = hotelAmenitiesRepository;
            _roomAmenitiesRepository = roomAmenitiesRepository;
            _roomTypeRepository = roomTypeRepository;
            _hotelGroupRepository = hotelGroupRepository;
        }

        public async Task<HotelBranchRoomDTO> AddBranchRoom(HotelBranchRoomDTO hotelBranchRoomDTO)
        {
            try
            {
                var GetHotelBranch = await _hotelBranchRepository.GetByIdAsync(hotelBranchRoomDTO.HotelBranchId);
                if (GetHotelBranch == null)
                {
                    throw new EmptyDataException("Hotel Branch not found");
                }
                var GetHotelGroup = await _hotelGroupRepository.GetByIdAsync(GetHotelBranch.HotelGroupId);
                if (GetHotelGroup == null)
                {
                    throw new EmptyDataException("Hotel Group not found");
                }
                GetHotelBranch.HotelGroup = GetHotelGroup;
                hotelBranchRoomDTO.RoomType.HotelBranchId = GetHotelBranch.HotelBranchId;
                hotelBranchRoomDTO.RoomAmenities.HotelBranchId = GetHotelBranch.HotelBranchId;
                hotelBranchRoomDTO.RoomType.HotelBranch = GetHotelBranch;
                hotelBranchRoomDTO.RoomAmenities.Hotel = GetHotelBranch;
                var GetDuplicateRoomType = await _roomTypeRepository.GetByBranchAndRoomType(hotelBranchRoomDTO.HotelBranchId, hotelBranchRoomDTO.RoomType.RoomTypeName);
                var AddBranchRoomType = new Models.Hotels.RoomType();
                if (GetDuplicateRoomType == null)
                {
                    AddBranchRoomType = await _roomTypeRepository.AddAsync(hotelBranchRoomDTO.RoomType);
                }
                else
                {
                    hotelBranchRoomDTO.RoomType.RoomTypeId = GetDuplicateRoomType.RoomTypeId;
                    AddBranchRoomType = await _roomTypeRepository.UpdateAsync(hotelBranchRoomDTO.RoomType); 
                }
                if (AddBranchRoomType == null)
                {
                    throw new ErrorInServiceException("Error: Room Type not added");
                }


                //Room Amenities

                hotelBranchRoomDTO.RoomAmenities.RoomTypeId = AddBranchRoomType.RoomTypeId;
                

                var DuplicateRoomAmenities = await _roomAmenitiesRepository.GetByBranchAndAmenity(hotelBranchRoomDTO.HotelBranchId, AddBranchRoomType.RoomTypeId);

                hotelBranchRoomDTO.RoomAmenities.RoomTypeId = AddBranchRoomType.RoomTypeId;
                hotelBranchRoomDTO.RoomAmenities.RoomType= AddBranchRoomType;
                if (DuplicateRoomAmenities != null)
                {
                    var UpdateRoomAmenities = await _roomAmenitiesRepository.UpdateAsync(hotelBranchRoomDTO.RoomAmenities);
                    if (UpdateRoomAmenities == null)
                    {
                        throw new ErrorInServiceException("Error: Room Amenities not updated");
                    }
                    return new HotelBranchRoomDTO
                    {
                        HotelBranchId = GetHotelBranch.HotelBranchId,
                        RoomType = AddBranchRoomType,
                        RoomAmenities = UpdateRoomAmenities
                    };
                }
                var AddBranchRoomAmenities = await _roomAmenitiesRepository.AddAsync(hotelBranchRoomDTO.RoomAmenities);
                if (AddBranchRoomAmenities == null)
                {
                    throw new ErrorInServiceException("Error: Room Amenities not added");
                }
                return new HotelBranchRoomDTO
                {
                    HotelBranchId = GetHotelBranch.HotelBranchId,
                    RoomType = AddBranchRoomType,
                    RoomAmenities = AddBranchRoomAmenities
                };
            }
            catch(Exception ex)
            {
                throw new ErrorInServiceException(ex.Message);
            }
        }

        public Task<HotelBranchDTO> AddNewHotelBranch(HotelBranchDTO addNewHotelBranchDTO)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<HotelBranchDTO> DeleteHotelBranch(int hotelBranchId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteRoomType(int RoomTypeId)
        {
            try
            {
                var DeleteRoomAmenities = await _roomAmenitiesRepository.DeleteAsync(RoomTypeId);
                var DeleteRoomType = await _roomTypeRepository.DeleteAsync(RoomTypeId);
                if (DeleteRoomType == false)
                {
                    throw new ErrorInServiceException("Error: Room Type not deleted");
                }
                return DeleteRoomType;
            }
            catch(Exception ex)
            {
                throw new ErrorInServiceException(ex.Message);
            }
        }

        public Task<IEnumerable<HotelBranchDTO>> GetAllHotelBranchUnderGroup()
        {
            throw new NotImplementedException();
        }

        public Task<HotelBranchDTO> GetHotelBranch(int hotelBranchId)
        {
            throw new NotImplementedException();
        }

        public Task<HotelBranchRoomDTO> UpdateBranchRoom(HotelBranchRoomDTO hotelBranchRoomDTO)
        {
            throw new NotImplementedException();
        }

        public async Task<HotelBranchDTO> UpdateHotelBranch(HotelBranchDTO HotelBranchDTO)
        {
            if(HotelBranchDTO == null)
            {
                throw new ArgumentNullException(nameof(HotelBranchDTO), "HotelBranch cannot be null");
            }
            try
            {
                var GetExistingDetails = await _hotelBranchRepository.GetByIdAsync(HotelBranchDTO.HotelBranchId);
                if (GetExistingDetails == null)
                {
                    throw new EmptyDataException("Hotel Branch not found");
                }


                var UpdateDemographics = new Models.Hotels.HotelDemographics();
                if (HotelBranchDTO.HotelDemographics != null)
                {
                    HotelBranchDTO.HotelDemographics.HotelId=GetExistingDetails.HotelBranchId;
                    HotelBranchDTO.HotelDemographics.Hotel=GetExistingDetails;
                    UpdateDemographics = await _hotelDemographicsRepository.UpdateAsync(HotelBranchDTO.HotelDemographics);
                }

                var UpdateRules = new Models.Hotels.HotelBranchRules();
                if (HotelBranchDTO.HotelBranchRules != null)
                {
                    HotelBranchDTO.HotelBranchRules.HotelBranchId = GetExistingDetails.HotelBranchId;
                    HotelBranchDTO.HotelBranchRules.Hotel=GetExistingDetails;
                    UpdateRules = await _hotelBranchRulesRepository.UpdateAsync(HotelBranchDTO.HotelBranchRules);
                }

                var UpdateAmenities = new Models.Hotels.HotelAmenities();
                if (HotelBranchDTO.HotelAmenities != null)
                {
                    HotelBranchDTO.HotelAmenities.HotelBranchId = GetExistingDetails.HotelBranchId;
                    HotelBranchDTO.HotelAmenities.Hotel=GetExistingDetails;
                    UpdateAmenities = await _hotelAmenitiesRepository.UpdateAsync(HotelBranchDTO.HotelAmenities);
                }

                return new HotelBranchDTO
                {
                    HotelBranchId = GetExistingDetails.HotelBranchId,
                    HotelDemographics = UpdateDemographics,
                    HotelBranchRules = UpdateRules,
                    HotelAmenities = UpdateAmenities
                };

            }
            catch(Exception ex)
            {
                throw new ErrorInServiceException($"Error: {ex.Message}");
            }
        }
    }
}