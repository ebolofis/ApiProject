namespace Pos_WebApi.Models.DTOModels
{
    public interface IDTOModel<TEntiy> where TEntiy : class
    {
        TEntiy ToModel();
        // void FromModel(TEntiy model);
        TEntiy UpdateModel(TEntiy model);
        //object[] GetKeys();
    }
}
