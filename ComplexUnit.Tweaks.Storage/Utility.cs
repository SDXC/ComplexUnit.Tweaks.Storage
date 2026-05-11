using System.Collections.Generic;
using System.Reflection;

using Mafi;
using Mafi.Core.Products;
using Mafi.Core.Prototypes;


namespace ComplexUnit.Tweaks.Storage
{
    internal static class Utility
    {
        internal static void AddToStorableProducts(ProtosDb protosDb, ProductProto.ID[] protos)
        {
            FieldInfo isStorableField = typeof(ProductProto).GetField("IsStorable", BindingFlags.Instance | BindingFlags.Public);
            List<ProductProto> foundProducts = new List<ProductProto>();

            foreach (ProductProto.ID id in protos)
            {
                Option<ProductProto> option = protosDb.Get<ProductProto>(id);

                if (option.HasValue)
                {
                    ProductProto product = option.Value;
                    foundProducts.Add(product);

                    if (isStorableField != null && !product.IsStorable)
                        isStorableField.SetValue(product, true);
                }
            }
        }
    }
}