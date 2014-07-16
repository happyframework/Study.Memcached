using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using Enyim.Caching;
using Enyim.Caching.Memcached;

namespace Study.Memcached.Tutorials
{
    [TestFixture]
    public sealed class MemcachedClientStudy
    {
        [Test]
        public void Set_Study()
        {
            using (var client = new MemcachedClient())
            {
                var result1 = client.Store(StoreMode.Set, "userid", "123456");
                Assert.True(result1);

                // 可以多次调用
                var result2 = client.Store(StoreMode.Set, "userid", "123456");
                Assert.True(result2);

                client.FlushAll();
            }
        }

        [Test]
        public void Add_Study()
        {
            using (var client = new MemcachedClient())
            {
                var result1 = client.Store(StoreMode.Add, "userid", "123456");
                Assert.True(result1);

                // 不可以多次调用
                var result2 = client.Store(StoreMode.Add, "userid", "123456");
                Assert.False(result2);

                client.FlushAll();
            }
        }

        [Test]
        public void Replace_Study()
        {
            using (var client = new MemcachedClient())
            {
                // 没有键，不能替换。
                var result1 = client.Store(StoreMode.Replace, "userid", "123456");
                Assert.False(result1);

                // 有的话，就可以。
                client.Store(StoreMode.Add, "userid", "123456");
                var result2 = client.Store(StoreMode.Replace, "userid", "123456");
                Assert.True(result2);

                client.FlushAll();
            }
        }

        [Test]
        public void Get_Study()
        {
            using (var client = new MemcachedClient())
            {
                // 没有键，返回null。
                var result1 = client.Get("userid");
                Assert.Null(result1);

                client.Store(StoreMode.Set, "userid", "123456");
                var result2 = client.Get("userid");
                Assert.AreEqual("123456", result2);

                client.FlushAll();
            }
        }

        [Test]
        public void Delete_Study()
        {
            using (var client = new MemcachedClient())
            {
                // 没有键，返回false。
                var result1 = client.Remove("userid");
                Assert.False(result1);

                client.Store(StoreMode.Set, "userid", "123456");
                var result2 = client.Remove("userid");
                Assert.True(result2);

                client.FlushAll();
            }
        }

        [Test]
        public void Gets_Study()
        {
            using (var client = new MemcachedClient())
            {
                /* 
                 * 非常重要的CAS操作 
                 */

                client.Store(StoreMode.Set, "userid", "123456");
                var result = client.GetWithCas("userid");

                Assert.AreEqual("123456", result.Result);
                Assert.Greater(result.Cas, 0u);

                client.FlushAll();
            }
        }

        [Test]
        public void Cas_Study()
        {
            using (var client = new MemcachedClient())
            {
                /* 
                 * 非常重要的CAS操作 
                 */

                client.Store(StoreMode.Set, "userid", "123456");
                var result1 = client.GetWithCas("userid");
                var result2 = client.Cas(StoreMode.Set, "userid", "6543321");
                Assert.True(result2.Result);
                var result3 = client.Cas(StoreMode.Set, "userid", "123456", result1.Cas);
                Assert.False(result3.Result);

                client.FlushAll();
            }
        }
    }
}
