using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHook
{
    /// <summary>フックインターフェース</summary>
    public interface IHook : IDisposable
    {
        /// <summary>フックを開始する</summary>
        /// <returns>各種アクションを登録するHookChainオブジェクト</returns>
        HookChain Set();
        /// <summary>フックを終了します。Disposeした場合にも実行されます。</summary>
        void UnSet();
    }
}
