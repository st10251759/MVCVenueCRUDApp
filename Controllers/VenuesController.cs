using MVCFirstBasicApp.Models;
using Microsoft.AspNetCore.Mvc;
using System;


namespace MVCFirstBasicApp.Controllers
{
    /// <summary>
    /// CONTROLLER (C in MVC)
    /// This class handles incoming web requests (URLs) related to Venues.
    ///
    /// Naming convention note (important):
    /// - In ASP.NET MVC, controllers are typically PascalCase and end with "Controller".
    ///   So this should be: VenuesController (plural + capital V).
    ///
    /// Why it matters:
    /// - It keeps your project consistent and predictable.
    /// - ASP.NET routing + scaffolding + teammates will expect this convention.
    /// </summary>
    public class VenuesController : Controller
    {
        // --------------------------------------------------------------------
        // TEMPORARY DATA STORAGE (In-Memory "Dummy Data")
        // --------------------------------------------------------------------
        // We're using a static list to simulate a database.
        //
        // WHY:
        // - You learn MVC flow first (Controller -> View, Model Validation, Routing)
        // - Without dealing with database setup yet (EF Core, migrations, etc.)
        //
        // IMPORTANT:
        // - This data resets when the app restarts.
        // - In a real app, we'd replace this with EF Core + a database.
        private static readonly List<Venue> _venues = new()
        {
            new Venue { id = 1, name = "Grand Hall", Address = "123 Main St", venueCapacity = 500 },
            new Venue { id = 2, name = "Conference Center", Address = "456 Elm St", venueCapacity = 1000 },
            new Venue { id = 3, name = "Outdoor Arena", Address = "789 Oak St", venueCapacity = 20000 }
        };

        // --------------------------------------------------------------------
        // GET: /Venues
        // --------------------------------------------------------------------
        /// <summary>
        /// GET /Venues (or /Venues/Index)
        ///
        /// WHAT THIS DOES:
        /// - Shows a page that lists all venues.
        ///
        /// WHY:
        /// - Index is the standard "list" page in MVC.
        /// - Users need a place to view what's stored.
        ///
        /// HOW IT CONNECTS:
        /// - This returns the View "Views/Venues/Index.cshtml"
        /// - We pass the venue list into that view using: View(_venues)
        /// - The view will loop through the list and display it as HTML.
        /// </summary>
        public IActionResult Index()
        {
            // Pass venues to the view so the UI can render them.
            return View(_venues);
        }

        // --------------------------------------------------------------------
        // GET: /Venues/Create
        // --------------------------------------------------------------------
        /// <summary>
        /// GET /Venues/Create
        ///
        /// WHAT THIS DOES:
        /// - Displays the empty "Create Venue" form.
        ///
        /// WHY:
        /// - Web forms are usually a two-step process:
        ///   1) GET action shows the form = reading
        ///   2) POST action handles the submitted form = writing 
        ///
        /// HOW IT CONNECTS:
        /// - This returns the view "Views/Venues/Create.cshtml"
        /// - The view contains inputs for Name, Address, VenueCapacity, etc.
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            // Return the Create view (empty form).
            return View();
        }

        // --------------------------------------------------------------------
        // POST: /Venues/Create
        // --------------------------------------------------------------------
        /// <summary>
        /// POST /Venues/Create
        ///
        /// WHAT THIS DOES:
        /// - Handles the form submission when the user clicks "Save".
        ///
        /// WHY:
        /// - POST requests are used for actions that change data (create/update/delete).
        ///
        /// MODEL BINDING (very important):
        /// - ASP.NET MVC automatically takes the form fields (like Name, Address)
        ///   and fills them into the Venue object parameter below.
        ///
        /// VALIDATION (very important):
        /// - ModelState.IsValid checks the DataAnnotation rules in your Venue model
        ///   e.g. [Required], [StringLength], [Range]
        ///
        /// REDIRECT (very important):
        /// - If we successfully add a venue, we redirect to Index.cshtml instead of returning the same View.
        /// - This prevents the "refresh and resubmit" problem (duplicate submissions).
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken] // Security: prevents CSRF attacks on your form posts
        public IActionResult Create(Venue venue)
        {
            // 1) Validate the incoming form data using the Venue model rules.
            if (!ModelState.IsValid)
            {
                // If invalid, return the same form view with the user's entered values.
                // The view will show validation errors next to the fields automatically.
                return View(venue);
            }

            // 2) Generate a new Id since we don't have a database auto-increment yet.
            //    - If list is empty -> start at 1
            //    - Else -> max Id + 1
            venue.id = _venues.Count == 0 ? 1 : _venues.Max(v => v.id) + 1;

            // 3) Add the new venue to our in-memory list (simulating an INSERT into a DB).
            _venues.Add(venue);

            // 4) Redirect to Index so the user sees the updated list.
            //    This also prevents duplicate form submissions if the user refreshes.
            return RedirectToAction(nameof(Index));
        }

        // GET: /Venues/Details/5
        public IActionResult Details(int id)
        {
            var venue = _venues.FirstOrDefault(v => v.id == id);
            if (venue == null) return NotFound();
            return View(venue);
        }
        // GET: /Venues/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var venue = _venues.FirstOrDefault(v => v.id == id);
            if (venue == null) return NotFound();
            return View(venue);
        }

        // POST: /Venues/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Venue venue)
        {
            if (!ModelState.IsValid) return View(venue);

            var existing = _venues.FirstOrDefault(v => v.id == venue.id);
            if (existing == null) return NotFound();

            // Update the properties in-place
            existing.name = venue.name;
            existing.Address = venue.Address;
            existing.venueCapacity = venue.venueCapacity;

            return RedirectToAction(nameof(Index));
        }

        // GET: /Venues/Delete/5
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var venue = _venues.FirstOrDefault(v => v.id == id);
            if (venue == null) return NotFound();
            return View(venue);
        }

        // POST: /Venues/DeleteConfirmed
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var venue = _venues.FirstOrDefault(v => v.id == id);
            if (venue != null) _venues.Remove(venue);
            return RedirectToAction(nameof(Index));
        }

        // --------------------------------------------------------------------
        // NOTE: Remove "Indexx"
        // --------------------------------------------------------------------
        // This action is not needed and will confuse routing and your project structure.
        // You already have Index() which is the correct list action.
        //
        // Keeping extra actions like Indexx() often causes:
        // - confusion when linking
        // - wrong view names
        // - routing issues
        //
        // If you wanted a second page, give it a meaningful name like:
        // - Details(int id)
        // - Edit(int id)
        // - Delete(int id)
        // not Indexx.
        //
        // public IActionResult Indexx()
        // {
        //     return View();
        // }
    }
}