using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolvenKit.Common.Services;

namespace WolvenKit.Common.Model.Cr2w
{
    #region REDtypes

    [Editor(typeof(IExpandableObjectEditor), typeof(IPropertyEditorBase))]
    public interface IEditableVariable
    {
    }




    public interface IREDPrimitive
    {

    }

    public interface IREDIntegerType : IREDPrimitive { }


    public interface IEditorBindable
    {
    }
    public interface IEditorBindable<T> : IEditorBindable
    {
        public T Value { get; set; } // ???
    }

    public interface IREDBool : IEditorBindable<bool>
    {
    }

    public interface IEnumAccessor : IEditorBindable
    {
       
    }

    public interface IEnumAccessor<T> : IEditorBindable<T>, IEnumAccessor where T : Enum
    {
        
        
    }

    public interface IArrayAccessor : IEditableVariable, IList
    {
       

    }

    public interface IChunkPtrAccessor
    {
    }

    
    public interface IArrayAccessor<T> : IArrayAccessor
    {
    }

    public interface IBufferAccessor : IArrayAccessor
    {

    }

    public interface IVariantAccessor
    {
    }

    public interface IBufferVariantAccessor : IVariantAccessor
    {
    }

    public interface IHandleAccessor : IChunkPtrAccessor
    {
       
    }

    

    #endregion

}
