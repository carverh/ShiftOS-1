using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Shop.
/// </summary>
namespace ShiftOS.Objects
{
	/// <summary>
	/// Shop.
	/// </summary>
    public class Shop
    {
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
        public string Name { get; set; }

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
        public string Description { get; set; }

		/// <summary>
		/// Gets or sets the items.
		/// </summary>
		/// <value>The items.</value>
        public List<ShopItem> Items { get; set; }
    }

    /// <summary>
	/// Shop item.
	/// </summary>
    public abstract class ShopItem
    {
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
        public string Name { get; set; }

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
        public string Description { get; set; }

		/// <summary>
		/// Gets or sets the cost.
		/// </summary>
		/// <value>The cost.</value>
        public int Cost { get; set; }

		/// <summary>
		/// Gets or sets the shop owner.
		/// </summary>
		/// <value>The shop owner.</value>
        public string ShopOwner { get; set; }

		/// <summary>
		/// Raises the buy event.
		/// </summary>
        protected abstract void OnBuy();

		/// <summary>
		/// Gives the CP to shop owner.
		/// </summary>
		/// <returns>The CP to shop owner.</returns>
		/// <param name="cp">Cp.</param>
        protected abstract void GiveCPToShopOwner(int cp);

		/// <summary>
		/// Buy the specified buyer.
		/// </summary>
		/// <param name="buyer">Buyer.</param>
        public bool Buy(ref Save buyer)
        {
            if(buyer.Codepoints >= Cost) // If user has enough codepoints
            {
                buyer.Codepoints -= Cost; // Subtract Codepoints
                OnBuy(); // Buy item
                GiveCPToShopOwner(Cost); // Pay shop owner
                return true; // A-OK
            }
            else
            {
                return false; // Not enough $$$
            }
        }
    }
}

// Comments by @carverh
