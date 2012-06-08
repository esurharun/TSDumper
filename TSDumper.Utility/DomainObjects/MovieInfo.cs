using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using TheMovieDB;

namespace DomainObjects
{
    public class MovieInfo
    {
        public string Name { get { return(name); } }
        public string AlternativeName { get { return (alternativeName); } }
        public string Budget { get { return (budget); } }
        public string MovieRuntime { get { return (movieRuntime); } }
        public string Overview { get { return (overview); } }
        public decimal Popularity { get { return (popularity); } }
        public decimal Rating { get { return (rating); } }
        public DateTime? Released { get { return (released); } }
        public string Revenue { get { return (revenue); } }
        public TimeSpan Runtime { get { return (runtime); } }
        public decimal Score { get { return (score); } }
        public Collection<string> Studios { get { return (studios); } }
        public bool Translated { get { return(translated); } }
        
        private string name;
        private string alternativeName;
        private string budget;
        private string movieRuntime;
        private string overview;
        private decimal popularity;
        private decimal rating;
        private DateTime? released;
        private string revenue;
        private TimeSpan runtime;
        private decimal score;
        private Collection<string> studios;
        private bool translated;

        private static TmdbAPI api;

        private MovieInfo() { }

        public MovieInfo(string name)
        {
            this.name = name;
        }

        public static Collection<MovieInfo> GetMovieInfo(string title)
        {
            if (api == null)
                api = new TmdbAPI("b5410cd85abf11ab7e32d6addd5d5963");

            TmdbMovie[] movies;

            try
            {
                movies = api.MovieSearch(title);
            }
            catch (Exception)
            {
                return (null);
            }

            if (movies == null)
                return (null);

            Collection<MovieInfo> results = new Collection<MovieInfo>();

            foreach (TmdbMovie movie in movies)
            {
                MovieInfo movieInfo = new MovieInfo(movie.Name);

                movieInfo.alternativeName = movie.AlternativeName;
                movieInfo.budget = movie.Budget;
                movieInfo.movieRuntime = movie.MovieRuntime;
                movieInfo.overview = movie.Overview;
                movieInfo.popularity = movie.Popularity;
                movieInfo.rating = movie.Rating;
                movieInfo.released = movie.Released;
                movieInfo.revenue = movie.Revenue;
                movieInfo.runtime = movie.Runtime;
                movieInfo.score = movie.Score;

                if (movie.Studios != null)
                {
                    movieInfo.studios = new Collection<string>();

                    foreach (TmdbStudio studio in movie.Studios)
                        movieInfo.studios.Add(studio.Name);
                }
                
                movieInfo.translated = movie.Translated;

                results.Add(movieInfo);
            }

            return (results);
        }
    }
}
