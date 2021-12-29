const setupNavbarBurger = () => {

    /*
     * Setup menu burger behaviour
     */

    // Code copied from Bulma documentation
    // https://bulma.io/documentation/components/navbar/#navbar-menu


    const $navbarBurgerDots = document.querySelector(".navbar-burger-dots");

    if ($navbarBurgerDots !== null) {
        $navbarBurgerDots.addEventListener("click", (ev) => {
            const $nacaraNavbarMenu = document.querySelector(".nacara-navbar-menu");

            if ($nacaraNavbarMenu !== null) {
                $nacaraNavbarMenu.classList.toggle("is-active");
                $navbarBurgerDots.classList.toggle("is-active");
            }
        });
    }

}

const setupMobileMenu = () => {

    const mobileMenuTrigger = document.querySelector(".mobile-menu .menu-trigger");

    if (mobileMenuTrigger !== null) {
        mobileMenuTrigger
            .addEventListener("click", () => {
                document
                    .querySelector(".is-menu-column")
                    .classList
                    .toggle("force-show");

                mobileMenuTrigger.classList.toggle("is-active");
            });
    }

}

// Setup the copy code button for snippets
const setupCopyCode = () => {
    const snippetElements = Array.from(document.querySelectorAll("pre > code"));

    snippetElements
        .forEach(codeElement => {
            // If one of the parent of codeElement has data-disable-copy-button attributes
            // do not had the copy button
            // We store this information on a parent because we don't control the `snippet` generation
            if (codeElement.closest("[data-disable-copy-button]")) {
                return;
            }

            const copyButton = document.createElement("button");
            copyButton.innerText = "Copy";
            copyButton.classList.add(
                "button",
                "is-primary",
                "is-outlined",
                "is-copy-button"
            );

            const codeText = codeElement.innerText;

            copyButton.addEventListener("click", () => {
                // Copy the code into the clipboard
                const $input = document.createElement("textarea");
                document.body.appendChild($input);
                $input.value = codeText;
                $input.select();
                document.execCommand("copy");
                $input.remove();

                // Notify the user
                copyButton.innerText = "Copied";
                // Revert the button text
                window.setTimeout(() => {
                    copyButton.innerText = "Copy";
                }, 1000)
            })

            codeElement.appendChild(copyButton);
        });
}

const setupNavbarDropdown = () => {
    document
        .querySelectorAll(".navbar-item.has-nacara-dropdown")
        .forEach(function (navbarItem) {
            navbarItem.addEventListener("click", (ev) => {
                // Click is inside the dropdown element, nothing to do
                if (ev.target.closest(".nacara-dropdown")) {
                    return;
                }

                const $activeDropdownItem = document.querySelector(".navbar-item.has-nacara-dropdown.is-active");

                // If we clicked on the active dropdown close it
                if ($activeDropdownItem) {
                    const activeItemGuid = $activeDropdownItem.attributes.getNamedItem("data-guid").value;
                    const clickedItemGuid = navbarItem.attributes.getNamedItem("data-guid").value;

                    // If user clicked on the active dropdown item, close everything
                    if (activeItemGuid === clickedItemGuid) {
                        // Close the dropdown
                        $activeDropdownItem.classList.remove("is-active");
                        // Remove the overlay
                        $greyOverlay.classList.remove("is-active");
                    } else {
                        // The user clicked on another dropdown item, close the active one
                        $activeDropdownItem.classList.remove("is-active");
                        // Show the new dropdown as active
                        navbarItem.classList.add("is-active");
                    }
                } else {
                    // Show the overlay
                    $greyOverlay.classList.add("is-active");
                    // Show which dropdown is active
                    navbarItem.classList.add("is-active");
                }
            });
        })

    const navbarDropdownTimeoutCache = {};

    // Fullwidth dropdown needs to handle the hover event via JavaScript to add a small delay
    // before hiding the dropdown when the mouse leave
    // This is because when displaying a dropdown in fullwidth there is a gap between
    // the dropdown-link and the dropdown content
    document.querySelectorAll(".navbar-item.has-nacara-dropdown.is-fullwidth")
        .forEach(function (navbarItem) {
            navbarItem.addEventListener("mouseenter", function() {
                const itemGuid = navbarItem.attributes.getNamedItem("data-guid").value;

                // If there is already a timeout for this dropdown cancel it
                if (navbarDropdownTimeoutCache[itemGuid]) {
                    clearTimeout(navbarDropdownTimeoutCache[itemGuid]);
                    delete navbarDropdownTimeoutCache[itemGuid];
                }

                // Add the CSS class which force to show the dropdown from the JS
                navbarItem.classList.add("is-hovered-js");

                // When the mouse leave, wait a small amount of time before removing the CSS class
                navbarItem.addEventListener("mouseleave", function(ev) {
                    const timeoutId = setTimeout(() => {
                        navbarItem.classList.remove("is-hovered-js");
                        delete navbarDropdownTimeoutCache[itemGuid];
                    }, 200);
                    // Store the timeoutID so we can cancel it if the user make some back and forth
                    // between the link and the dropdown content
                    navbarDropdownTimeoutCache[itemGuid] = timeoutId;
                }, { once : true});
            })
        })

    const $greyOverlay = document.querySelector(".grey-overlay");

    if ($greyOverlay !== null) {
        $greyOverlay.addEventListener("click", (ev) => {
            const $activeDropdownItem = document.querySelector(".navbar-item.has-nacara-dropdown.is-active");

            if ($activeDropdownItem) {
                $activeDropdownItem.classList.remove("is-active");
                $greyOverlay.classList.remove("is-active");
            }
        });
    }
}

const scrollMenuOrTableOfContentIfNeeded = () => {

    // Make the table of content visible
    const $tableOfContentElement = document.querySelector(".table-of-content");
    const $activeMenuItemElement = document.querySelector(".menu .is-active");

    if ($tableOfContentElement !== null) {
        scrollIntoView($tableOfContentElement, {
            scrollMode: "if-needed",
            block: 'nearest',
            inline: 'nearest',
            boundary: document.querySelector(".menu-container")
        });
    } else if ($activeMenuItemElement !== null) {
        scrollIntoView($activeMenuItemElement, {
            scrollMode: "if-needed",
            block: 'nearest',
            inline: 'nearest',
            boundary: document.querySelector(".menu-container")
        });
    }
}

const setupGlobal = () => {

    // This script should be loaded in a tag with async tag so we can directly apply all the functions

    setupNavbarBurger();
    setupNavbarDropdown();
    setupMobileMenu();
    setupCopyCode();
    scrollMenuOrTableOfContentIfNeeded();
}

// The page is ready to execute our code
if (document.readyState === "complete") {
    setupGlobal();
    // The page is not ready, wait for it to be ready
} else {
    document.onreadystatechange = () => {
        if (document.readyState === "complete") {
            setupGlobal();
        }
    }
}
