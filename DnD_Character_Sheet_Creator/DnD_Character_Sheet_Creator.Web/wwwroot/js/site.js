// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(function ($) {
	$(function () {
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
