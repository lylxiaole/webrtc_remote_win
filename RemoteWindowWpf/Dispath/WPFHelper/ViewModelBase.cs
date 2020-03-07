using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Controls;

namespace Dispath
{
    /// 重写属性更改通知基类
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo, IDisposable
    {
        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }
        ~ViewModelBase()
        {
            Dispose(false);
        }

        #region 资源释放
        protected virtual void Dispose(bool disposing)
        {
        }
        #endregion

        #region 数据校验
        private Dictionary<string, string> errors = new Dictionary<string, string>();

        public bool HasErrors
        {
            get
            {
                return errors.Count > 0;
            }
        }
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return "";
            }
            else
            {
                if (errors.ContainsKey(propertyName))
                {
                    return new List<string> { errors[propertyName] };
                }
                else
                {
                    return null;
                }
            }
        }

        protected void HaveErrors(string errorInfo, [CallerMemberName]string callerName = null)
        {
             
            errors.Remove(callerName);
            errors.Add(callerName, errorInfo);
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(callerName));
        }

        protected void ClearErrors([CallerMemberName]string callerName = null)
        { 
            errors.Remove(callerName);
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(callerName));
        }

        #endregion

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string callerName = null)
        { 
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(callerName));
            }
        }
         



        #endregion


       
    }

    public static class MemberControlHelper
    {
        public static void OnPropertyChanged<T, TProperty>(this T baseModel, Expression<Func<T, TProperty>> expression) where T : ViewModelBase
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression != null)
            {
                string propertyName = memberExpression.Member.Name;
                baseModel.OnPropertyChanged();
            }
            else
                throw new NotImplementedException();
        }
    }

}
