using Data.Models;
using Service.Interface;
using System.Diagnostics;

namespace Service.Service
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        PostgresContext _postgresContext;

        Repository<User> _repositoryUser;
        Repository<UserBan> _repositoryUserBan;
        Repository<UserStatusLog> _repositoryUserStatusLog;
        Repository<Status> _repositoryStatus;
        Repository<Cart> _repositoryCart;
        Repository<Order> _repositoryOrder;
        Repository<OrderItem> _repositoryOrderItem;
        Repository<OrderStatus> _repositoryOrderStatus;
        Repository<OrderStatusLog> _repositoryOrderStatusLog;
        Repository<Box> _repositoryBox;
        Repository<BoxItem> _repositoryBoxItem;
        Repository<Product> _repositoryProduct;
        Repository<ProductVariant> _repositoryProductVariant;
        Repository<Feedback> _repositoryFeedback;
        Repository<Tag> _repositoryTag;
        Repository<ProductTag> _repositoryProductTag;
        Repository<BoxTag> _repositoryBoxTag;
        Repository<Media> _repositoryMedia;
        Repository<TagValue> _repositoryTagValue;
        Repository<MediaType> _repositoryMediaType;
        Repository<BoxMedia> _repositoryBoxMedia;
        Repository<ProductMedia> _repositoryProductMedia;
        Repository<FeedbackMedia> _repositoryFeedbackMedia;
        Repository<PaymentDetail> _repositoryPaymentDetail;
        Repository<PaymentStatus> _repositoryPaymentStatus;
       
        Repository<Store> _repositoryStore;
        Repository<StoreItem> _repositoryStoreItem;
        Repository<StoreMember> _repositoryStoreMember;

        Repository<Voucher> _repositoryVoucher;

        Repository<Size> _repositorySize;
        Repository<Color> _repositoryColor;
        Repository<Brand> _repositoryBrand;
        



        private bool disposedValue;

        public UnitOfWork(PostgresContext postgresContext)
        {
            _postgresContext = postgresContext;
        }

        public Repository<User> RepositoryUser {  get { return _repositoryUser ??= new Repository<User>(_postgresContext); } }
        public Repository<UserBan> RepositoryUserBan {  get { return _repositoryUserBan ??= new Repository<UserBan>(_postgresContext); } }

        public Repository<UserStatusLog> RepositoryUserStatusLog { get { return _repositoryUserStatusLog ??= new Repository<UserStatusLog>(_postgresContext); } }
        public Repository<Status> RepositoryStatus { get { return _repositoryStatus ??= new Repository<Status>(_postgresContext); } }
        public Repository<Cart> RepositoryCart { get { return _repositoryCart ??= new Repository<Cart>(_postgresContext); } }
        public Repository<Order> RepositoryOrder { get { return _repositoryOrder ??= new Repository<Order>(_postgresContext); } }
        public Repository<OrderItem> RepositoryOrderItem { get { return _repositoryOrderItem ??= new Repository<OrderItem>(_postgresContext); } }
        public Repository<OrderStatus> RepositoryOrderStatus { get { return _repositoryOrderStatus ??= new Repository<OrderStatus>(_postgresContext); } }
        public Repository<OrderStatusLog> RepositoryOrderStatusLog  { get { return _repositoryOrderStatusLog ??= new Repository<OrderStatusLog>(_postgresContext); } }
        public Repository<Box> RepositoryBox { get { return _repositoryBox ??= new Repository<Box>(_postgresContext); } }
        public Repository<BoxItem> RepositoryBoxItem { get { return _repositoryBoxItem ??= new Repository<BoxItem>(_postgresContext); } }
        public Repository<Product> RepositoryProduct { get { return _repositoryProduct ??= new Repository<Product>(_postgresContext); } }
        public Repository<ProductVariant> RepositoryProductVariant { get { return _repositoryProductVariant ??= new Repository<ProductVariant>(_postgresContext); } }
        public Repository<Feedback> RepositoryFeedback { get { return _repositoryFeedback ??= new Repository<Feedback>(_postgresContext); } }
        public Repository<Tag> RepositoryTag { get { return _repositoryTag ??= new Repository<Tag>(_postgresContext); } }
        public Repository<ProductTag> RepositoryProductTag { get { return _repositoryProductTag ??= new Repository<ProductTag>(_postgresContext); } }
        public Repository<BoxTag> RepositoryBoxTag { get { return _repositoryBoxTag ??= new Repository<BoxTag>(_postgresContext); } }
        public Repository<Media> RepositoryMedia { get { return _repositoryMedia ??= new Repository<Media>(_postgresContext); } }
        public Repository<TagValue> RepositoryTagValue { get { return _repositoryTagValue ??= new Repository<TagValue>(_postgresContext); } }
        public Repository<MediaType> RepositoryMediaType { get { return _repositoryMediaType ??= new Repository<MediaType>(_postgresContext); } }
        public Repository<BoxMedia> RepositoryBoxMedia { get { return _repositoryBoxMedia ??= new Repository<BoxMedia>(_postgresContext); } }
        public Repository<ProductMedia> RepositoryProductMedia { get { return _repositoryProductMedia ??= new Repository<ProductMedia>(_postgresContext); } }
        public Repository<FeedbackMedia> RepositoryFeedbackMedia { get { return _repositoryFeedbackMedia ??= new Repository<FeedbackMedia>(_postgresContext); } }
        public Repository<Size> RepositorySize { get { return _repositorySize ??= new Repository<Size>(_postgresContext); } }
        public Repository<Color> RepositoryColor { get { return _repositoryColor ??= new Repository<Color>(_postgresContext); } }
        public Repository<Brand> RepositoryBrand { get { return _repositoryBrand ??= new Repository<Brand>(_postgresContext); } }

        public Repository<Store> repositoryStore { get { return _repositoryStore ??= new Repository<Store>(_postgresContext);}}
        public Repository<StoreItem> repositoryStoreItem { get { return _repositoryStoreItem ??= new Repository<StoreItem>(_postgresContext);}}
        public Repository<StoreMember> repositoryStoreMember { get { return _repositoryStoreMember  ??= new Repository<StoreMember>(_postgresContext); } }

        public Repository<Voucher> repositoryVoucher { get { return _repositoryVoucher ??= new Repository<Voucher>(_postgresContext); } }


        public Repository<PaymentStatus> RepositoryPaymentStatus { get { return _repositoryPaymentStatus ??= new Repository<PaymentStatus>(_postgresContext); } }
        public Repository<PaymentDetail> RepositoryPaymentDetail { get { return _repositoryPaymentDetail ??= new Repository<PaymentDetail>(_postgresContext); } }

        public async Task CommitAsync()
        {
            await _postgresContext.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _postgresContext.Dispose();
                }

               
                disposedValue = true;
            }
        }

       

        public void Dispose()
        {
         
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
