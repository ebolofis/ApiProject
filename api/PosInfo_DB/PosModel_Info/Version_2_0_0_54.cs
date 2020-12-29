using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium_DTOs.PosModel_Info
{
    [DisplayName("Ver : 2.0.0.54")]
    public class Version_2_0_0_54
    {
        public List<string> Ver_2_0_0_54 { get; }

        public Version_2_0_0_54()
        {
            Ver_2_0_0_54 = new List<string>();

            Ver_2_0_0_54.Add("CREATE TRIGGER dbo.OrderStatusOnKds ON  dbo.OrderStatus AFTER INSERT \n"
                           + "AS  \n"
                           + "BEGIN \n"
                           + "	DECLARE @OrderId BIGINT, @Status BIGINT \n"
                           + "	SELECT @OrderId = OrderId, @Status = Status FROM INSERTED  \n"
                           + "	 \n"
                           + "	IF (@Status <> 1) \n"
                           + "		DELETE FROM OrderDetailIgredientsKDS WHERE OrderId = @OrderId \n"
                           + "	ELSE \n"
                           + "		INSERT INTO OrderDetailIgredientsKDS(OrderId,OrderDetailId,ProductId,IgredientsId, \n"
                           + "			[Description],Qty,SalesTypeId,KDSId) \n"
                           + "		SELECT fn.OrderId, fn.OrderDetailId, fn.ProductId, fn.IngredientsId, \n"
                           + "			   fn.IngredientsDescription, fn.Qty, fn.SalesTypeId, fn.KdsId \n"
                           + "		FROM ( \n"
                           + "			SELECT lst.OrderId, lst.OrderDetailId, lst.ProductId, lst.IngredientsId, lst.IngredientsDescription, SUM(lst.Qty) Qty, lst.SalesTypeId, lst.KdsId  \n"
                           + "			FROM ( \n"
                           + "				SELECT od.OrderId, od.Id OrderDetailId, od.ProductId ,i.Id IngredientsId, i.[Description] IngredientsDescription, od.Qty, od.SalesTypeId, od.KdsId  \n"
                           + "				FROM OrderDetail AS od \n"
                           + "				INNER JOIN ProductRecipe AS pr ON pr.ProductId = od.ProductId \n"
                           + "				INNER JOIN Ingredients AS i ON i.Id = pr.IngredientId AND ISNULL(i.DisplayOnKds,0) <> 0 \n"
                           + "				WHERE od.OrderId = @OrderId \n"
                           + "				UNION ALL \n"
                           + "				SELECT od.OrderId, od.Id OrderDetailId, od.ProductId ,i.Id IngredientsId, i.[Description] IngredientsDescription, (odi.Qty * od.Qty) Qty, od.SalesTypeId, od.KdsId \n"
                           + "				FROM OrderDetail AS od \n"
                           + "				INNER JOIN OrderDetailIgredients AS odi ON odi.OrderDetailId = od.Id \n"
                           + "				INNER JOIN Ingredients AS i ON i.Id = odi.IngredientId AND ISNULL(i.DisplayOnKds,0) <> 0 \n"
                           + "				WHERE od.OrderId = @OrderId \n"
                           + "			) lst \n"
                           + "			GROUP BY lst.OrderId, lst.OrderDetailId, lst.ProductId, lst.IngredientsId, lst.IngredientsDescription,lst.SalesTypeId, lst.KdsId  \n"
                           + "		) fn \n"
                           + "		WHERE fn.Qty > 0 \n"
                           + "END");
        }
    }
}
