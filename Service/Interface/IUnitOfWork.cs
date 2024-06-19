using Data.Models;
using Service.Service;

public interface IUnitOfWork : IDisposable
{
    Repository<User> RepositoryUser { get; }
    Repository<UserStatusLog> RepositoryUserStatusLog { get; }
    Repository<Status> RepositoryStatus { get; }
    Repository<Cart> RepositoryCart { get; }
    Repository<Order> RepositoryOrder { get; }
    Repository<OrderItem> RepositoryOrderItem { get; }
    Repository<OrderStatus> RepositoryOrderStatus { get; }
    Repository<OrderStatusLog> RepositoryOrderStatusLog { get; }
    Repository<Box> RepositoryBox { get; }
    Repository<BoxItem> RepositoryBoxItem { get; }
    Repository<Product> RepositoryProduct { get; }
    Repository<ProductVariant> RepositoryProductVariant { get; }
    Repository<Feedback> RepositoryFeedback { get; }
    Repository<Tag> RepositoryTag { get; }
    Repository<ProductTag> RepositoryProductTag { get; }
    Repository<BoxTag> RepositoryBoxTag { get; }
    Repository<Media> RepositoryMedia { get; }
    Repository<TagValue> RepositoryTagValue { get; }
    Repository<MediaType> RepositoryMediaType { get; }
    Repository<BoxMedia> RepositoryBoxMedia { get; }
    Repository<ProductMedia> RepositoryProductMedia { get; }
    Repository<FeedbackMedia> RepositoryFeedbackMedia { get; }
    Repository<Size> RepositorySize { get; }
    Repository<Color> RepositoryColor { get; }
    Repository<Brand> RepositoryBrand { get; }
    Repository<Voucher> RepositoryVoucher { get; }
    Repository<Voucher> repositoryVoucher { get; }
    Repository<PaymentDetail> RepositoryPaymentDetail { get; }
    Repository<PaymentStatus> RepositoryPaymentStatus { get; }
    Repository<StoreMember> repositoryStoreMember { get; }
    Repository<StoreItem> repositoryStoreItem { get; }
    Repository<Store> repositoryStore { get; }
    Repository<UserBan> RepositoryUserBan { get; }

    Task CommitAsync();
    public interface IUnitOfWork
    {
        Repository<User> RepositoryUser { get; }
        Repository<UserStatusLog> RepositoryUserStatusLog { get; }
        Repository<Status> RepositoryStatus { get; }
        Repository<Box> RepositoryBox { get; }
        Repository<BoxItem> RepositoryBoxItem { get; }
        Repository<BoxMedia> RepositoryBoxMedia { get; }
        Repository<BoxTag> RepositoryBoxTag { get; }
        Repository<Cart> RepositoryCart { get; }
        Repository<Feedback> RepositoryFeedback { get; }
        Repository<FeedbackMedia> RepositoryFeedbackMedia { get; }
        Repository<Media> RepositoryMedia { get; }
        Repository<MediaType> RepositoryMediaType { get; }
        Repository<Order> RepositoryOrder { get; }
        Repository<OrderItem> RepositoryOrderItem { get; }
        Repository<OrderStatus> RepositoryOrderStatus { get; }
        Repository<OrderStatusLog> RepositoryOrderStatusLog { get; }
        Repository<Product> RepositoryProduct { get; }
        Repository<ProductMedia> RepositoryProductMedia { get; }
        Repository<ProductTag> RepositoryProductTag { get; }
        Repository<ProductVariant> RepositoryProductVariant { get; }
        Repository<Tag> RepositoryTag { get; }
        Repository<TagValue> RepositoryTagValue { get; }
        Repository<Size> RepositorySize { get; }
        Repository<Color> RepositoryColor { get; }
        Repository<Brand> RepositoryBrand { get; }
        Repository<PaymentStatus> RepositoryPaymentStatus { get; }
        Repository<PaymentDetail> RepositoryPaymentDetail { get; }
        Repository<Store> repositoryStore { get; }
        Repository<StoreItem> repositoryStoreItem { get; }
        Repository<StoreMember> repositoryStoreMember { get; }
        Repository<Voucher> repositoryVoucher { get; }
        Repository<UserBan> RepositoryUserBan { get; }

        Task CommitAsync();
    }
}