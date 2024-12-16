using System.ComponentModel;

namespace Mozaodol.Domain.Entities.MessageEntities
{
    public enum MediaType
    {
        None,
        [Description("image")]
        Image,
        [Description("video")]
        Video,
        [Description("audio")]
        Audio
    }
}
