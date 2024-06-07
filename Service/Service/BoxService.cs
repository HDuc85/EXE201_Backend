using Data.Models;
using Data.ViewModel;
using Data.ViewModel.Box;
using Microsoft.AspNetCore.Identity;
using Service.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Service
{
    public class BoxService : IBoxService
    {
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public BoxService(UserManager<User> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<bool>> CreateBox(CreateBoxRequest request)
        {
            var box = new Box
            {
                BoxName = request.BoxName,
                Price = request.Price,
                Thumbnail = request.Thumbnail,
                Discount = request.Discount
            };
            await _unitOfWork.RepositoryBox.Insert(box);
            await _unitOfWork.CommitAsync();
            return new ApiResult<bool> { Success = true, message = "Box created successfully" };
        }

        public async Task<ApiResult<BoxViewModel>> GetBoxById(int boxId)
        {
            var box = await _unitOfWork.RepositoryBox.GetById(boxId);
            if (box == null)
                return new ApiResult<BoxViewModel> { Success = false, message = "Box not found" };

            var boxViewModel = new BoxViewModel
            {
                Id = box.Id,
                BoxName = box.BoxName,
                Price = box.Price,
                Thumbnail = box.Thumbnail,
                Discount = box.Discount
            };
            return new ApiResult<BoxViewModel> { Success = true, Value = boxViewModel };
        }

        public async Task<ApiResult<List<BoxViewModel>>> GetAllBoxes()
        {
            var boxes = await _unitOfWork.RepositoryBox.GetAll();
            var boxViewModels = boxes.Select(box => new BoxViewModel
            {
                Id = box.Id,
                BoxName = box.BoxName,
                Price = box.Price,
                Thumbnail = box.Thumbnail,
                Discount = box.Discount
            }).ToList();

            return new ApiResult<List<BoxViewModel>> { Success = true, Value = boxViewModels };
        }

        public async Task<ApiResult<bool>> UpdateBox(UpdateBoxRequest request)
        {
            var box = await _unitOfWork.RepositoryBox.GetById(request.Id);
            if (box == null)
                return new ApiResult<bool> { Success = false, message = "Box not found" };

            box.BoxName = request.BoxName;
            box.Price = request.Price;
            box.Thumbnail = request.Thumbnail;
            box.Discount = request.Discount;

            _unitOfWork.RepositoryBox.Update(box);
            await _unitOfWork.CommitAsync();
            return new ApiResult<bool> { Success = true, message = "Box updated successfully" };
        }

        public async Task<ApiResult<bool>> DeleteBox(int boxId)
        {
            var box = await _unitOfWork.RepositoryBox.GetById(boxId);
            if (box == null)
                return new ApiResult<bool> { Success = false, message = "Box not found" };

            _unitOfWork.RepositoryBox.Delete(box);
            await _unitOfWork.CommitAsync();
            return new ApiResult<bool> { Success = true, message = "Box deleted successfully" };
        }

    }
}
