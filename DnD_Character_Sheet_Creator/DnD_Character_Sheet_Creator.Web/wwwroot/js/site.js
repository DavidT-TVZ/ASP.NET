// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(function ($) {
	$(function () {
		function formatDateTimeControls(root) {
			if (!window.Intl || !Intl.DateTimeFormat) {
				return;
			}

			var scope = root || document;
			var dateTimeControls = scope.querySelectorAll("[data-date-time-control='true']");

			if (!dateTimeControls.length) {
				return;
			}

			var browserLocale = (navigator.languages && navigator.languages.length ? navigator.languages[0] : navigator.language) || document.documentElement.lang || "en-US";
			var dateTimeFormatter = new Intl.DateTimeFormat(browserLocale, {
				dateStyle: "medium",
				timeStyle: "short"
			});

			dateTimeControls.forEach(function (control) {
				var isoValue = control.getAttribute("data-date-time-iso") || control.getAttribute("datetime");

				if (!isoValue) {
					return;
				}

				var date = new Date(isoValue);

				if (isNaN(date.getTime())) {
					return;
				}

				var formatted = dateTimeFormatter.format(date);

				control.textContent = formatted;
				control.setAttribute("title", formatted);
				control.setAttribute("datetime", date.toISOString());
			});
		}

		window.formatDateTimeControls = formatDateTimeControls;

		var scrollOpenings = document.querySelectorAll("[data-scroll-opening]");

		if (scrollOpenings.length) {
			var isReducedMotion = window.matchMedia && window.matchMedia("(prefers-reduced-motion: reduce)").matches;

			scrollOpenings.forEach(function (opening) {
				var sheet = opening.querySelector(".scroll-opening__sheet");

				function setHeight() {
					if (!sheet) return;
					opening = opening; // keep reference
					var h = sheet.scrollHeight;
					opening.style.setProperty("--scroll-opening-height", h + "px");
				}

				if (sheet) {
					setHeight();

					// recompute on resize with debounce
					var resizeTimer = null;
					window.addEventListener('resize', function () {
						clearTimeout(resizeTimer);
						resizeTimer = setTimeout(setHeight, 150);
					});
				}

				opening.classList.add("is-preparing");

				window.requestAnimationFrame(function () {
					window.setTimeout(function () {
						opening.classList.add("is-open");
					}, isReducedMotion ? 0 : 120);
				});
			});
		}

		formatDateTimeControls(document);

		$(".ajax-list-search-shell").each(function () {
			var $shell = $(this);
			var searchUrl = $shell.data("search-url");
			var autocompleteUrl = $shell.data("autocomplete-url");
			var $input = $shell.find(".ajax-list-search-input");
			var $autocomplete = $shell.find(".ajax-list-search-autocomplete");
			var $listRegion = $shell.nextAll(".ajax-list-search-region").first();
			var countBadgeSelector = $shell.data("count-badge");
			var $countBadge = countBadgeSelector ? $(countBadgeSelector) : $();
			var searchTimer = null;
			var searchRequest = null;
			var autocompleteRequest = null;

			if (!$input.length || !$autocomplete.length || !$listRegion.length) {
				return;
			}

			function updateCount(html) {
				var $markup = $("<div>").html(html);
				var count = $markup.find(".ajax-list-search-item").length;

				if ($countBadge.length) {
					$countBadge.text(count + " shown");
				}
			}

			function hideAutocomplete() {
				$autocomplete.addClass("d-none").empty();
			}

			function renderAutocomplete(items) {
				if (!items || !items.length) {
					hideAutocomplete();
					return;
				}

				var markup = items.map(function (item) {
					return '<button type="button" class="list-group-item list-group-item-action ajax-list-search-autocomplete__item" data-value="' + item.value + '">' + item.label + '</button>';
				}).join("");

				$autocomplete.html(markup).removeClass("d-none");
			}

			function runSearch(query) {
				if (searchRequest) {
					searchRequest.abort();
				}

				searchRequest = $.ajax({
					url: searchUrl,
					method: "GET",
					data: { query: query }
				}).done(function (html) {
					$listRegion.html(html);
					formatDateTimeControls($listRegion[0]);
					updateCount(html);
				});
			}

			function runAutocomplete(term) {
				if (autocompleteRequest) {
					autocompleteRequest.abort();
				}

				autocompleteRequest = $.ajax({
					url: autocompleteUrl,
					method: "GET",
					data: { term: term }
				}).done(function (items) {
					renderAutocomplete(items);
				});
			}

			$input.on("input", function () {
				var term = $(this).val().trim();

				window.clearTimeout(searchTimer);

				if (!term) {
					hideAutocomplete();
					runSearch("");
					return;
				}

				searchTimer = window.setTimeout(function () {
					runSearch(term);
					if (term.length >= 2) {
						runAutocomplete(term);
					} else {
						hideAutocomplete();
					}
				}, 220);
			});

			$autocomplete.on("click", ".ajax-list-search-autocomplete__item", function () {
				var value = $(this).data("value");

				$input.val(value);
				hideAutocomplete();
				runSearch(value);
			});

			$(document).on("click", function (event) {
				if (!$(event.target).closest(".ajax-list-search-shell").length) {
					hideAutocomplete();
				}
			});
		});
	});
})(jQuery);
