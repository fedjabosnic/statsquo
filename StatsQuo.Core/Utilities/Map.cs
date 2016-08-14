using System.Collections.Generic;

namespace StatsQuo.Core.Utilities
{
	/// <summary>
	/// A map is a special type of dictionary where indexing with a non-existent key will
	/// yield a default value (rather than throwing an exception like the default dictionary).
	/// </summary>
	/// <example>
	/// <code>
	/// var map = new Map&lt;string, Map&lt;string, Map&lt;string, int&gt;&gt;&gt;();
	/// 
	/// // Keys a, b and c do not exist
	/// var x = map["a"]["b"]["c"];
	/// 
	/// x.Should().Be(0);
	/// </code>
	/// </example>
	/// <typeparam name="TKey">The key type</typeparam>
	/// <typeparam name="TValue">The value type</typeparam>
	public class Map<TKey, TValue> : Dictionary<TKey, TValue> where TValue : new()
	{
		public new TValue this[TKey key]
		{
			get
			{
				TValue value;
				return TryGetValue(key, out value) ? value : base[key] = new TValue();
			}
			set
			{
				base[key] = value;
			}
		}
	}
}