namespace Lens.Core.App.Web.Options;

public interface IControllerOptions
{
    /// <summary>
    /// This causes controller results to be left as is. 
    /// The result that is returned to the front-end is not guarenteed to be an IResultModel.
    /// </summary>
    IControllerOptions IgnoreResultModelWrapping();
    /// <summary>
    /// This causes the Json Serializer to treat enums as strings while parsing.
    /// </summary>
    IControllerOptions JsonSerializeEnumsAsStrings();
    /// <summary>
    /// When the value of an object is null, this property will not be serialized at all.
    /// </summary>
    IControllerOptions JsonSerializeIgnoreNullProperties();
    /// <summary>
    /// Will also register services to work with razor-views.
    /// </summary>
    IControllerOptions UseViews();
}