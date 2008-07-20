//
// Taken from Troy Goode's blog http://www.squaredroot.com/post/2008/06/MVC-and-SSL.aspx
//
//
namespace Suteki.Common.Extensions
{
    /// <summary>
    /// Provides helper extensions for turning strings into fully-qualified and SSL-enabled Urls.
    /// </summary>
    public static class UrlStringExtensions
    {

        /// <summary>
        /// Takes a relative or absolute url and returns the fully-qualified url path.
        /// </summary>
        /// <param name="text">The url to make fully-qualified. Ex: Home/About</param>
        /// <returns>The absolute url plus protocol, server, & port. Ex: http://localhost:1234/Home/About</returns>
        public static string ToFullyQualifiedUrl(this string text)
        {
            return new UrlExtensionsHelper().ToFullyQualifiedUrl(text);
        }

        /// <summary>
        /// Looks for Html links in the passed string and turns each relative or absolute url and returns the fully-qualified url path.
        /// </summary>
        /// <param name="text">The url to make fully-qualified. Ex: <a href="Home/About">Blah</a></param>
        /// <returns>The absolute url plus protocol, server, & port. Ex: <a href="http://localhost:1234/Home/About">Blah</a></returns>
        public static string ToFullyQualifiedLink(this string text)
        {
            return new UrlExtensionsHelper().ToFullyQualifiedLink(text);
        }

        /// <summary>
        /// Takes a relative or absolute url and returns the fully-qualified url path using the Https protocol.
        /// </summary>
        /// <param name="text">The url to make fully-qualified. Ex: Home/About</param>
        /// <returns>The absolute url plus server, & port using the Https protocol. Ex: https://localhost:1234/Home/About</returns>
        public static string ToSslUrl(this string text)
        {
            return new UrlExtensionsHelper().ToSslUrl(text);
        }

        /// <summary>
        /// Looks for Html links in the passed string and turns each relative or absolute url into a fully-qualified url path using the Https protocol.
        /// </summary>
        /// <param name="text">The url to make fully-qualified. Ex: <a href="Home/About">Blah</a></param>
        /// <returns>The absolute url plus server, & port using the Https protocol. Ex: <a href="https://localhost:1234/Home/About">Blah</a></returns>
        public static string ToSslLink(this string text)
        {
            return new UrlExtensionsHelper().ToSslLink(text);
        }
    }
}
