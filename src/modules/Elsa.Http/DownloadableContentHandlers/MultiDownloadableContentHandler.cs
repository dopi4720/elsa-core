using System.Collections;
using Elsa.Http.Abstractions;
using Elsa.Http.Contexts;

namespace Elsa.Http.DownloadableContentHandlers;

/// <summary>
/// Handles content that represents a list of downloadable objects.
/// </summary>
public class MultiDownloadableContentHandler : DownloadableContentHandlerBase
{
    /// <inheritdoc />
    public override bool GetSupportsContent(object content) => content is IEnumerable and not string and not byte[];

    /// <inheritdoc />
    protected override IEnumerable<Func<ValueTask<Downloadable>>> GetDownloadablesAsync(DownloadableContext context)
    {
        var collectedDownloadables = new List<Func<ValueTask<Downloadable>>>();
        var content = context.Content;
        var enumerable = (IEnumerable) content;
        var manager = context.Manager;

        foreach (var item in enumerable)
        {
            var downloadables = manager.GetDownloadablesAsync(item, context.Options, context.CancellationToken);
            collectedDownloadables.AddRange(downloadables);
        }

        return collectedDownloadables;
    }
}