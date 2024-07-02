using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data.Models;
using Service.Interface;
using Service.Repo;
using Data.ViewModel.Product;
using Data.ViewModel;
using static System.Net.Mime.MediaTypeNames;
using Data.ViewModel.User;
using Firebase.Auth;
using Service.Helper;
using Data.ViewModel.Tag;
using System.Linq;

namespace Service.Service.System.Tag
{
    public class TagService : ITagService
    {
        private readonly UnitOfWork _unitOfWork;

        public TagService(UnitOfWork unitOfWork)
        {

            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<bool>> CreateTag(CreateTagDTO createTagDTO)
        {
            if (createTagDTO == null || string.IsNullOrEmpty(createTagDTO.TagName))
            {
                return new ApiResult<bool> { Success = false, message = "Invalid tag data" };
            }

            var tag = new Data.Models.Tag
            {
                TagName = createTagDTO.TagName,
                IsActive = true,
                TagValues = createTagDTO.TagValues?.Select(tv => new TagValue
                {
                    Value = tv
                }).ToList()
            };

            _unitOfWork.RepositoryTag.Insert(tag);
            await _unitOfWork.CommitAsync();
            return new ApiResult<bool> { Success = true, message = "Tag created successfully" };
        }

        public async Task<ApiResult<bool>> DeleteTag(int Id)
        {
            var tag = await _unitOfWork.RepositoryTag.GetById(Id);
            if (tag == null)
            {
                return new ApiResult<bool> { Success = false, message = "Tag not found" };
            }

            var tagvalue = await _unitOfWork.RepositoryTagValue.GetListByCondition(c => c.TagId == Id);

            if (tagvalue == null || !tagvalue.Any())
            {
                return new ApiResult<bool> { Success = false, message = "Tag values not found" };
            }

            var tagValueIds = tagvalue.Select(tv => tv.Id).ToList();

            var productTags = await _unitOfWork.RepositoryProductTag.GetListByCondition(pt => pt.TagVauleId.HasValue && tagValueIds.Contains(pt.TagVauleId.Value));
            foreach (var productTag in productTags)
            {
                productTag.IsActive = false;
                _unitOfWork.RepositoryProductTag.Update(productTag);
            }

            //_unitOfWork.RepositoryTagValue.RemoveRange(tagvalue);
            tag.IsActive = false;
            _unitOfWork.RepositoryTag.Update(tag);
            await _unitOfWork.CommitAsync();

            return new ApiResult<bool> { Success = true, message = "Tag deleted successfully" };
        }


        public async Task<ApiResult<bool>> UpdateTag(UpdateTagDTO updateTagDTO, int id)
        {
            if (updateTagDTO == null || id <= 0 || string.IsNullOrEmpty(updateTagDTO.TagName))
            {
                return new ApiResult<bool> { Success = false, message = "Invalid tag data" };
            }

            var tag = await _unitOfWork.RepositoryTag.GetById(id);
            if (tag == null)
            {
                return new ApiResult<bool> { Success = false, message = "Tag not found" };
            }

            tag.TagName = updateTagDTO.TagName;
            tag.IsActive = true;

            // Handle tag values update (assuming you have logic to handle the update of TagValues)
            if (updateTagDTO.TagValues != null)
            {
                tag.TagValues.Clear();
                foreach (var tv in updateTagDTO.TagValues)
                {
                    tag.TagValues.Add(new TagValue { Value = tv });
                }
            }

            _unitOfWork.RepositoryTag.Update(tag);
            await _unitOfWork.CommitAsync();

            return new ApiResult<bool> { Success = true, message = "Tag updated successfully" };
        }
    }
}
