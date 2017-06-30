using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHook
{
    /// <summary>
    /// 任意の型でInvokeを行われた際に自動的に登録されているアクションを読み出すクラスです。
    /// </summary>
    public class HookChain : IDisposable
    {
        /// <summary>登録するクラスの最終処理</summary>
        IDisposable _disposer;
        /// <summary>型とアクションのディクショナリ</summary>
        Dictionary<Type, object> _methods = new Dictionary<Type, object>();

        /// <summary>このクラスを利用する側の最終処理を設定してインスタンスを生成します。</summary>
        /// <param name="disposer">最終処理</param>
        public HookChain(IDisposable disposer)
        {
            _disposer = disposer;
        }

        /// <summary>自動で登録されている型にマッチするアクションを読み出します。</summary>
        /// <typeparam name="T">呼び出す登録されている型</typeparam>
        /// <param name="obj">呼び出すアクションへ渡すオブジェクト</param>
        public void Invoke<T>(T obj)
        {
            // 登録されているメソッド一覧を読み出す
            _methods?.ToList().ForEach(kvp =>
            {
                // アクションの引数の型と引数の型が同じかどうかを比較
                if (typeof(T) == kvp.Key)
                {
                    // マッチするアクションが見つかったらオブジェクトを渡して実行
                    Action<T> method = (Action<T>)kvp.Value;

                    method?.Invoke((T)obj);
                }

                // object型の登録なら型に関係なく実行
                if (typeof(object) == kvp.Key)
                {
                    Action<object> method = (Action<object>)kvp.Value;

                    method?.Invoke(obj);
                }
            });
        }

        /// <summary>
        /// 指定の引数型でアクションを登録
        /// </summary>
        /// <typeparam name="T">登録する型</typeparam>
        /// <param name="action">登録するアクション</param>
        /// <returns>自分自身を返します</returns>
        public HookChain Is<T>(Action<T> action)
        {
            // 既に同じ型で登録されていない場合のみ登録する
            if (!_methods.ContainsKey(typeof(T)))
                _methods.Add(typeof(T), action);

            // 自分自身を返す
            return this;
        }

        /// <summary>
        /// 任意の引数型でアクションを登録
        /// </summary>
        /// <param name="action">登録するアクション</param>
        /// <returns>自分自身を返します</returns>
        public HookChain Any(Action<object> action) => Is<object>(action);

        public void Dispose()
        {
            // アクション一覧の情報を開放する
            _methods.Clear();
            // 呼び出し元の最終処理を実行する
            _disposer.Dispose();
        }
    }
}
