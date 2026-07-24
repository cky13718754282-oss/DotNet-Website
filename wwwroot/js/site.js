document.addEventListener("DOMContentLoaded", function () {
    var root = document.documentElement;
    var themeToggle = document.getElementById("themeToggle");
    var themeIcon = document.getElementById("themeIcon");

    function updateThemeIcon() {
        if (!themeIcon) return;
        themeIcon.textContent = root.getAttribute("data-bs-theme") === "dark" ? "☀" : "☾";
    }

    updateThemeIcon();

    if (themeToggle) {
        themeToggle.addEventListener("click", function () {
            var nextTheme = root.getAttribute("data-bs-theme") === "dark" ? "light" : "dark";
            root.setAttribute("data-bs-theme", nextTheme);
            localStorage.setItem("geekspace-theme", nextTheme);
            updateThemeIcon();
        });
    }

    document.querySelectorAll("[data-character-count]").forEach(function (field) {
        var output = field.closest("form")?.querySelector("[data-character-output]");
        function updateCount() {
            if (output) output.textContent = field.value.length.toString();
        }
        field.addEventListener("input", updateCount);
        updateCount();
    });

    document.querySelectorAll("form[data-confirm]").forEach(function (form) {
        form.addEventListener("submit", function (event) {
            if (!window.confirm(form.dataset.confirm)) {
                event.preventDefault();
            }
        });
    });
});
