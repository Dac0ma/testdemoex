//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace podgotovkaDemo
{
    using System;
    using System.Collections.Generic;
    
    public partial class partner
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public partner()
        {
            this.order_request = new HashSet<order_request>();
        }
    
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public Nullable<int> inn { get; set; }
        public Nullable<int> rating { get; set; }
        public Nullable<int> partner_type_id { get; set; }
        public Nullable<int> director_id { get; set; }
        public string login { get; set; }
        public string password { get; set; }
    
        public virtual director director { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<order_request> order_request { get; set; }
        public virtual partner_type partner_type { get; set; }
    }
}
