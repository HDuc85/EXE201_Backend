using Data.Models;
using Data.ViewModel;
using Data.ViewModel.Cart;
using Data.ViewModel.Store;
using Microsoft.AspNetCore.Http.HttpResults;
using Service.Helper.Media;
using Service.Interface;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Security.Authentication;

namespace Service.Service
{
    public class StoreService : IStoreService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IMediaHelper _mediaHelper;

        public  StoreService(IUnitOfWork unitOfWork, IUserService userService, IMediaHelper mediaHelper)
        {
            _mediaHelper = mediaHelper;
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public async Task<StoreVM> CreateStore(string username,StoreCreateRequest request)
        {   
            var user = await _userService.UserExits(username);
            if(request == null) throw new ArgumentNullException("Nothing to create");
            var avatarurl = await _mediaHelper.SaveMedia(request.Avatar, "Store");

            var store = new Store()
            {
                Address = request.Address,
                Avatar = avatarurl.url,
                Description = request.Description,
                Phone = request.Phone,
                Status = "Active"
            };
            await _unitOfWork.repositoryStore.Insert(store);
            await _unitOfWork.CommitAsync();

            await _unitOfWork.repositoryStoreMember.Insert(new StoreMember
            {
                MemberId = user.Id,
                StoreId = store.Id,
                IsActive = true,
            });
            await _unitOfWork.CommitAsync();

            return new StoreVM
            {
                Address= store.Address,
                Avatar= store.Avatar,
                Description= store.Description,
                Id= store.Id,
                Phone= store.Phone,
                Status= store.Status,
                ProductQuantity= store.ProductQuantity,
                Rate= store.Rate,
                StoreName = store.StoreName
            };
        }
        public async Task<ApiResult> InsertItem(string username, ItemRequest request)
        {
            await Authority(username, request.StoreId);
            var storeItems = await _unitOfWork.repositoryStoreItem.GetAll(x => x.StoreId == request.StoreId);
            List<StoreItem> items = new List<StoreItem>();

            if (request.BoxsId.Count > 0 && request.BoxsId != null)
            {
                foreach( var box in request.BoxsId)
                {
                    if(storeItems.SingleOrDefault(x => x.BoxId == box) == null)
                    {
                    var boxexist = await _unitOfWork.RepositoryBox.GetById(box);
                        if(boxexist != null)
                        {
                            items.Add(new StoreItem()
                            {
                                ProductId = boxexist.Id,
                                StoreId = request.StoreId,
                                IsActive = true,
                            });
                        }
                    }

                }
            }
            if (request.productVId.Count > 0 && request.productVId != null)
            {
                foreach (var producv in request.productVId)
                {
                    if (storeItems.SingleOrDefault(x => x.ProductId == producv) == null)
                    {
                        var productvExist = await _unitOfWork.RepositoryProductVariant.GetById(producv);
                        if (productvExist != null)
                        {
                            items.Add(new StoreItem()
                            {
                                ProductId = productvExist.Id,
                                StoreId = request.StoreId,
                                IsActive = true,
                            });
                        }
                    }

                }
            }

            if(items.Count > 0)
            {
                await _unitOfWork.repositoryStoreItem.Insert(items);
                await _unitOfWork.CommitAsync();
               
                return new()
                {
                    Success = true,
                    message = "Add Succefully"
                };
            
            }

            return new()
            {
                message = "Nothing added",
                Success = false
            };

        }

        public async Task<ApiResult> InsetMember(string username , MemberRequest request)
        {
            await Authority(username, request.StoreId);
            List<StoreMember> memberIds = new List<StoreMember>();
            foreach(var member in request.users)
            {
                var memberid = await _userService.FindByEmail(member);
                if (memberid == null)
                    memberid = await _userService.FindByUsername(member);
                   
                if(memberid != null)
                {
                    var memberExits = await _unitOfWork.repositoryStoreMember.
                         GetSingleByCondition(x => x.MemberId == memberid.Id && x.StoreId == request.StoreId);

                    if(memberExits == null)
                       {
                        await _userService.UpdateRole(new Data.ViewModel.User.UpdateRoleRequest
                        {
                            Username = memberid.UserName,
                            roles = ["staff"],
                        });
                        memberIds.Add(new StoreMember
                        {
                            StoreId = request.StoreId,
                            MemberId = memberid.Id,
                            IsActive = true
                        });
                    }
                }
            }

            if(memberIds.Count > 0)
            {
                await _unitOfWork.repositoryStoreMember.Insert(memberIds);
                await _unitOfWork.CommitAsync();
                return new()
                {
                    Success = true,
                    message = "Add Succefully"
                };
            }
            return new()
            {
                message = "Nobody added",
                Success = false
            };



        }
        public async Task<IEnumerable<StoreVM>> GetAll()
        {
            var stores =  await  _unitOfWork.repositoryStore.GetAll();

            return stores.Select(x => new StoreVM
            {
                Address = x.Address,
                Avatar = x.Avatar,
                Description = x.Description,
                Id = x.Id,
                Phone = x.Phone,
                ProductQuantity = x.ProductQuantity,
                Rate = x.Rate,
                Status = x.Status,
                StoreName = x.StoreName
            });
        }
        public async Task<ApiResult> UpdateStore(string username, UpdateStoreRequest request)
        {
            await Authority(username,request.StoreId);

            var store = await _unitOfWork.repositoryStore.GetById(request.StoreId);
            if(request.avatar != null) 
            {
              var url = await _mediaHelper.SaveMedia(request.avatar,"Store");
                store.Avatar = url.url;
            }
            if(request.StoreName != null)
            {
                store.StoreName = request.StoreName;
            }
            if(request.Address != null)
            {
                store.Address = request.Address;
            }
            if(request.Description != null)
            {
                store.Description = request.Description;
            }
            if(request.Status != null)
            {
                store.Status = request.Status;
            }
            if(request.Phone != null)
            {
                store.Phone = request.Phone;
            }
             _unitOfWork.repositoryStore.Update(store);
            await _unitOfWork.CommitAsync();

            return new ApiResult()
            {
                message = "update succesful",
                Success = true
            };
        }
        public async Task<StoreVM> GetById(int id)
        {
            var store = await _unitOfWork.repositoryStore.GetById(id);

            return new() {
            Status = store.Status,
            StoreName = store.StoreName,
            Address = store.Address,
            Avatar = store.Avatar,
            Description = store.Description,
            Id= store.Id,
            Phone= store.Phone,
            ProductQuantity= store.ProductQuantity,
            Rate = store.Rate,  
            };
        }

        public async Task<MemberStoreVM> GetAllMember(string username, int storeId)
        {
            var user = await Authority(username,storeId);

             
            var storeMember = await _unitOfWork.repositoryStoreMember.GetAll(x => 
            x.StoreId == storeId && x.MemberId != user.Id);

            if(storeMember != null)
            {
                List<Member> members = new List<Member>();
                foreach(var member in storeMember)
                {
                    var memberprofile = await _userService.FindById((Guid)member.MemberId);
                    if(memberprofile != null)
                    {
                        members.Add(new Member
                        {
                            Address = memberprofile.Address,
                            Avatar = memberprofile.Avatar,
                            Birthday = memberprofile.Birthday,
                            Email = memberprofile.Email,
                            Name = $"{memberprofile.Firstname} {memberprofile.Lastname}",
                            Phone = memberprofile.PhoneNumber,
                        });
                    }
                }

                return new MemberStoreVM()
                {
                    members = members,
                    StoreId = storeId,
                };
            }
            return null;
        }
    
        public async Task<ItemStoreVm> GetAllItemStore(string username, int storeId)
        {
             await Authority(username, storeId);
          
            var storeitems = await _unitOfWork.repositoryStoreItem.GetAll(x => x.StoreId == storeId && x.IsActive == true);

            if (storeitems != null)
            {
                List<PVariantViewModel> pvs = new List<PVariantViewModel>();
                List<BoxViewModel> boxs = new List<BoxViewModel>();

                foreach (var item in storeitems)
                {

                    if (item.BoxId != null)
                    {
                        var box = await _unitOfWork.RepositoryBox.GetById(item.BoxId);
                        boxs.Add(new BoxViewModel
                        {
                            Discount = box.Discount,
                            Name = box.BoxName,
                            Price = box.Price,
                            Thumbnail = box.Thumbnail,
                        });
                    }
                    if (item.ProductId != null)
                    {
                        PVariantViewModel pvViewModel = new PVariantViewModel();
                        var productv = await _unitOfWork.RepositoryProductVariant.GetById(item.ProductId);

                        var product = await _unitOfWork.RepositoryProduct.GetById(productv.Id);
                        if (productv.ColorId != null)
                        {
                            var color = await _unitOfWork.RepositoryColor.GetById(productv.ColorId);
                            pvViewModel.Color = color.ColorValue;
                        }
                        if (productv.BrandId != null)
                        {
                            var brand = await _unitOfWork.RepositoryBrand.GetById(productv.BrandId);
                            pvViewModel.Brand = brand.BrandValue;
                        }
                        if (productv.SizeId != null)
                        {
                            var size = await _unitOfWork.RepositorySize.GetById(productv.SizeId);
                            pvViewModel.Size = size.SizeValue;
                        }

                        pvViewModel.Thumbnail = productv.Thumbnail;
                        pvViewModel.Price = productv.Price;
                        pvViewModel.Discount = productv.Discount;
                        pvViewModel.Weight = pvViewModel.Weight;
                        pvViewModel.Name = product.ProductName;
                        pvs.Add(pvViewModel);
                    }
                }

                return new ItemStoreVm()
                {
                    Id = storeId,
                    boxViewModels = boxs,
                    pVariantViewModels = pvs,
                };
            }

            return null;
        }

        public async Task<User> Authority(string username, int storeId)
        {
            var user = await _userService.UserExits(username);

            var checkAdmin = await _userService.FindByEmail(username);
            if (checkAdmin == null)
                checkAdmin = await _userService.FindByUsername(username);

            var check = await _unitOfWork.repositoryStoreMember.GetSingleByCondition(x => x.StoreId ==  storeId && x.MemberId == checkAdmin.Id);
            if (check == null)
            {
                throw new NullReferenceException("you do not have enough authority");
            };

            
            return user;
        }
        
        public async Task<ApiResult> DeleteStore(string username, int storeId)
        {
            await Authority(username,storeId);

            var store = await _unitOfWork.repositoryStore.GetById(storeId);
            if (store == null)
            {
                return new ApiResult()
                {
                    message = "Store is invalid",
                    Success = false
                };
            }
            _unitOfWork.repositoryStoreItem.Delete(x => x.StoreId == storeId);
            _unitOfWork.repositoryStoreMember.Delete(x => x.StoreId == storeId);
            _unitOfWork.repositoryStore.Delete(store);
            await _unitOfWork.CommitAsync();
            return new ApiResult()
            {
                Success = true,
                message = "Deleted Store id"
            };

        }
        public async Task<ApiResult> DeleteStoreItem(string username, ItemRequest items)
        {
            await Authority(username, items.StoreId);

            var store = await _unitOfWork.repositoryStore.GetById(items.StoreId);
            if (store == null)
            {
                return new ApiResult()
                {
                    message = "Store is invalid",
                    Success = false
                };
            }
            if(items.productVId.Count > 0 && items.productVId != null) 
            foreach(var item in items.productVId)
            {
                _unitOfWork.repositoryStoreItem.Delete(x => x.ProductId == item && x.StoreId == store.Id);
            }
            
            if (items.BoxsId.Count > 0 && items.BoxsId != null)
            foreach (var item in items.BoxsId)
            {
                _unitOfWork.repositoryStoreItem.Delete(x => x.BoxId == item && x.StoreId == store.Id);
            }

            await _unitOfWork.CommitAsync();
            return new ApiResult()
            {
                Success = true,
                message = "Deleted Succesful"
            };

        }
        public async Task<ApiResult> DeleteStoreMember(string username, MemberRequest request)
        {
            await Authority(username, request.StoreId);

            var store = await _unitOfWork.repositoryStore.GetById(request.StoreId);
            if (store == null)
            {
                return new ApiResult()
                {
                    message = "Store is invalid",
                    Success = false
                };
            }
            if(request.users.Count > 0 && request.users != null)
                foreach( var user in request.users)
                {
                    var memberid = await _userService.FindByEmail(user);
                    if (memberid == null)
                        memberid = await _userService.FindByUsername(user);
                    if (memberid != null)
                        _unitOfWork.repositoryStoreMember.Delete(x => x.MemberId == memberid.Id && x.StoreId == request.StoreId);
                }


            await _unitOfWork.CommitAsync();

            if (request.users.Count > 0 && request.users != null)
                foreach (var user in request.users)
                {
                    var memberid = await _userService.FindByEmail(user);
                    if (memberid == null)
                        memberid = await _userService.FindByUsername(user);
                    if (memberid != null){
                        var member = await _unitOfWork.repositoryStoreMember.GetSingleByCondition(x => x.MemberId == memberid.Id);
                        if (member == null)
                        {
                            await _userService.RemoveRole((Guid)memberid.Id, ["staff"]);
                        }
                    }
                }

            return new ApiResult()
            {
                Success = true,
                message = "Deleted member"
            };

        }
    }
}
