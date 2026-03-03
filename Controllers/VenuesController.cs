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

        // --------------------------------------------------------------------
        // GET: /Venues/Details/5
        // --------------------------------------------------------------------
        /// <summary>
        /// GET /Venues/Details/{id}
        ///
        /// WHAT THIS DOES:
        /// - Displays a read-only page showing all properties of a single venue.
        ///
        /// WHY:
        /// - Users need a way to view the full information of one specific venue
        ///   without entering edit mode.
        /// - Separating Details from Edit is a safety measure — reading data
        ///   should never accidentally trigger a write.
        ///
        /// THE id PARAMETER (very important):
        /// - ASP.NET routing extracts the id from the URL: /Venues/Details/5
        /// - It is automatically mapped into the "int id" parameter below.
        ///   This is called ROUTE BINDING.
        ///
        /// NotFound() (very important):
        /// - If no venue matches the given id, we return a 404 Not Found response.
        /// - This prevents crashing and gives the browser a proper HTTP error code.
        ///
        /// HOW IT CONNECTS:
        /// - This returns the view "Views/Venues/Details.cshtml"
        /// - We pass the matching Venue object into the view using: View(venue)
        /// </summary>
        public IActionResult Details(int id)
        {
            // 1) Search the in-memory list for a venue whose id matches the route id.
            //    FirstOrDefault returns null if no match is found (simulates a DB lookup).
            var venue = _venues.FirstOrDefault(v => v.id == id);

            // 2) If no venue was found, return a 404 Not Found HTTP response.
            //    This handles bad URLs like /Venues/Details/999 gracefully.
            if (venue == null) return NotFound();

            // 3) Pass the found venue to the Details view for display.
            return View(venue);
        }

        // --------------------------------------------------------------------
        // GET: /Venues/Edit/5
        // --------------------------------------------------------------------
        /// <summary>
        /// GET /Venues/Edit/{id}
        ///
        /// WHAT THIS DOES:
        /// - Displays the Edit form pre-filled with the existing venue data.
        ///
        /// WHY:
        /// - Just like Create, editing is a two-step process:
        ///   1) GET loads the form with the current values so the user can see and change them.
        ///   2) POST (below) handles saving the changes.
        /// - Pre-filling the form improves usability — the user only changes what they need to.
        ///
        /// HOW IT CONNECTS:
        /// - This returns the view "Views/Venues/Edit.cshtml"
        /// - We pass the existing Venue object into the view.
        /// - The view uses asp-for tag helpers to bind each field to the model properties,
        ///   which automatically fills in the current values.
        /// </summary>
        [HttpGet]
        public IActionResult Edit(int id)
        {
            // 1) Find the venue to edit by its id.
            var venue = _venues.FirstOrDefault(v => v.id == id);

            // 2) Return 404 if the venue does not exist.
            if (venue == null) return NotFound();

            // 3) Pass the existing venue data to the Edit view so the form is pre-filled.
            return View(venue);
        }

        // --------------------------------------------------------------------
        // POST: /Venues/Edit/5
        // --------------------------------------------------------------------
        /// <summary>
        /// POST /Venues/Edit/{id}
        ///
        /// WHAT THIS DOES:
        /// - Receives the submitted edit form and updates the venue in the list.
        ///
        /// WHY:
        /// - POST requests are used for actions that change data.
        /// - We update the existing object in-place rather than removing and re-adding it,
        ///   so that the id is preserved (simulating an UPDATE query in a real database).
        ///
        /// MODEL BINDING (very important):
        /// - ASP.NET MVC maps the submitted form fields into the Venue parameter.
        /// - The hidden <input asp-for="id" /> field in the view ensures the id
        ///   is included in the POST so we know which venue to update.
        ///
        /// VALIDATION (very important):
        /// - ModelState.IsValid checks all DataAnnotation rules ([Required], [StringLength], [Range]).
        /// - If invalid, we return the same form view so the user can correct their input.
        ///
        /// REDIRECT (very important):
        /// - On success, we redirect to Index to show the updated list.
        /// - This prevents the "refresh and resubmit" duplicate update problem.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken] // Security: prevents CSRF attacks on form posts
        public IActionResult Edit(Venue venue)
        {
            // 1) Validate the submitted form data against the Venue model rules.
            if (!ModelState.IsValid)
            {
                // If invalid, return the Edit view with the user's entered values
                // so validation error messages display next to the relevant fields.
                return View(venue);
            }

            // 2) Find the existing venue in the list by id.
            //    The id was preserved via the hidden field in the Edit form.
            var existing = _venues.FirstOrDefault(v => v.id == venue.id);

            // 3) Return 404 if somehow the venue no longer exists (edge case safety check).
            if (existing == null) return NotFound();

            // 4) Update each property in-place on the existing object.
            //    This simulates an UPDATE statement in a real database.
            //    We do NOT replace the whole object so the id stays intact.
            existing.name = venue.name;
            existing.Address = venue.Address;
            existing.venueCapacity = venue.venueCapacity;

            // 5) Redirect to Index so the user sees the refreshed venue list.
            return RedirectToAction(nameof(Index));
        }

        // --------------------------------------------------------------------
        // GET: /Venues/Delete/5
        // --------------------------------------------------------------------
        /// <summary>
        /// GET /Venues/Delete/{id}
        ///
        /// WHAT THIS DOES:
        /// - Displays a confirmation page showing the venue details before deletion.
        ///
        /// WHY:
        /// - Deletion is irreversible, so we show the user exactly what they are
        ///   about to delete and ask them to confirm.
        /// - This is a standard safety pattern — never delete on a GET request.
        ///   GET requests should always be read-only (no data changes).
        ///
        /// HOW IT CONNECTS:
        /// - This returns the view "Views/Venues/Delete.cshtml"
        /// - We pass the matching Venue object into the view for display.
        /// - The view contains a form that POSTs to DeleteConfirmed to trigger the actual deletion.
        /// </summary>
        [HttpGet]
        public IActionResult Delete(int id)
        {
            // 1) Find the venue to delete by its id.
            var venue = _venues.FirstOrDefault(v => v.id == id);

            // 2) Return 404 if the venue does not exist.
            if (venue == null) return NotFound();

            // 3) Pass the venue to the Delete view so the user can review it before confirming.
            return View(venue);
        }

        // --------------------------------------------------------------------
        // POST: /Venues/DeleteConfirmed
        // --------------------------------------------------------------------
        /// <summary>
        /// POST /Venues/DeleteConfirmed
        ///
        /// WHAT THIS DOES:
        /// - Permanently removes the venue from the in-memory list.
        ///
        /// WHY IT IS NAMED "DeleteConfirmed" (very important):
        /// - C# does not allow two methods with the same name AND the same parameter types.
        /// - We already have DELETE(int id) as a GET above.
        /// - If we named this POST method Delete(int id) too, the compiler would throw an error.
        /// - The [ActionName("DeleteConfirmed")] attribute tells ASP.NET MVC that this method
        ///   should respond to the URL /Venues/DeleteConfirmed, keeping the routing clean.
        ///
        /// WHY DELETE ON POST (very important):
        /// - Deleting on a GET request is dangerous — search engines, browser prefetchers,
        ///   and accidental link clicks could trigger deletions unintentionally.
        /// - POST requires a deliberate form submission, which is the correct HTTP pattern
        ///   for any action that permanently changes or removes data.
        ///
        /// REDIRECT (very important):
        /// - After deletion we redirect to Index so the user sees the updated list
        ///   and cannot accidentally re-trigger the delete by refreshing the page.
        /// </summary>
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken] // Security: prevents CSRF attacks on form posts
        public IActionResult DeleteConfirmed(int id)
        {
            // 1) Find the venue in the list by its id.
            var venue = _venues.FirstOrDefault(v => v.id == id);

            // 2) Remove it if found.
            //    The null check protects against edge cases where the venue
            //    was already deleted in another session (since this is in-memory).
            if (venue != null) _venues.Remove(venue);

            // 3) Redirect to Index so the user sees the updated venue list.
            return RedirectToAction(nameof(Index));
        }
    }
}
