//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace CalendarApp
{
    using System;
    using System.Collections.Generic;
    
    public partial class events
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public Nullable<int> recurrence_id { get; set; }
        public Nullable<int> category_id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public System.DateTime start_time { get; set; }
        public System.DateTime end_time { get; set; }
        public Nullable<int> notify_time_before { get; set; }
        public string notify_status { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
    
        public virtual category category { get; set; }
        public virtual recurrence recurrence { get; set; }
        public virtual users users { get; set; }
    }
}
