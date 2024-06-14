using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class UserBan
{
    public int Id { get; set; }

    public DateTime? endDate {  get; set; }
    public Guid? UserId { get; set; }


}
