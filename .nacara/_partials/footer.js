import React from 'react';

const SitemapSection = ({
  title,
  children
}) => /*#__PURE__*/React.createElement("div", {
  className: "sitemap-section"
}, /*#__PURE__*/React.createElement("div", {
  className: "sitemap-section-title"
}, title), /*#__PURE__*/React.createElement("ul", {
  className: "sitemap-section-list"
}, children));

const SitemapSectionItem = ({
  text,
  icon,
  url
}) => /*#__PURE__*/React.createElement("li", null, /*#__PURE__*/React.createElement("a", {
  href: url,
  className: "icon-text sitemap-section-list-item"
}, /*#__PURE__*/React.createElement("span", {
  className: "icon"
}, /*#__PURE__*/React.createElement("i", {
  className: icon
})), /*#__PURE__*/React.createElement("span", {
  className: "sitemap-section-list-item-text"
}, text)));

const CopyrightScript = () => /*#__PURE__*/React.createElement("script", {
  dangerouslySetInnerHTML: {
    __html: `
        const year = new Date().getFullYear();
        document.getElementById('copyright-end-year').innerHTML = year;
        `
  }
});

export default /*#__PURE__*/React.createElement("div", {
  className: "is-size-5"
}, /*#__PURE__*/React.createElement("div", {
  className: "sitemap"
}, /*#__PURE__*/React.createElement(SitemapSection, {
  title: "Project ressources"
}, /*#__PURE__*/React.createElement(SitemapSectionItem, {
  text: "Repository",
  icon: "fas fa-file-code",
  url: "https://github.com/elmish/elmish"
}), /*#__PURE__*/React.createElement(SitemapSectionItem, {
  text: "Release notes",
  icon: "fas fa-list",
  url: "/elmish/release_notes.html"
}), /*#__PURE__*/React.createElement(SitemapSectionItem, {
  text: "License",
  icon: "fas fa-id-card",
  url: "https://github.com/elmish/elmish/blob/v4.x/LICENSE.md"
})), /*#__PURE__*/React.createElement(SitemapSection, {
  title: "Elmish modules"
}, /*#__PURE__*/React.createElement(SitemapSectionItem, {
  text: "Fable.Elmish",
  icon: "fa fa-book",
  url: "https://elmish.github.io/elmish/"
}), /*#__PURE__*/React.createElement(SitemapSectionItem, {
  text: "Fable.Elmish.Browser",
  icon: "fa fa-book",
  url: "https://elmish.github.io/browser/"
}), /*#__PURE__*/React.createElement(SitemapSectionItem, {
  text: "Fable.Elmish.UrlParser",
  icon: "fa fa-book",
  url: "https://elmish.github.io/urlParser/"
}), /*#__PURE__*/React.createElement(SitemapSectionItem, {
  text: "Fable.Elmish.Debugger",
  icon: "fa fa-book",
  url: "https://elmish.github.io/debugger/"
}), /*#__PURE__*/React.createElement(SitemapSectionItem, {
  text: "Fable.Elmish.React",
  icon: "fa fa-book",
  url: "https://elmish.github.io/react/"
}), /*#__PURE__*/React.createElement(SitemapSectionItem, {
  text: "Fable.Elmish.HMR",
  icon: "fa fa-book",
  url: "https://elmish.github.io/hmr/"
})), /*#__PURE__*/React.createElement(SitemapSection, {
  title: "Other Links"
}, /*#__PURE__*/React.createElement(SitemapSectionItem, {
  text: "Fable",
  icon: "faf faf-fable",
  url: "https://fable.io"
}), /*#__PURE__*/React.createElement(SitemapSectionItem, {
  text: "Fable Gitter",
  icon: "fab fa-gitter",
  url: "https://gitter.im/fable-compiler/Fable"
}), /*#__PURE__*/React.createElement(SitemapSectionItem, {
  text: "F# Slack",
  icon: "fab fa-slack",
  url: "https://fsharp.org/guides/slack/"
}), /*#__PURE__*/React.createElement(SitemapSectionItem, {
  text: "F# Software Foundation",
  icon: "faf faf-fsharp-org",
  url: "https://fsharp.org/"
}))), /*#__PURE__*/React.createElement("p", {
  className: "has-text-centered"
}, "Built with ", /*#__PURE__*/React.createElement("a", {
  className: "is-underlined",
  href: "https://mangelmaxime.github.io/Nacara/"
}, "Nacara")), /*#__PURE__*/React.createElement("p", {
  className: "has-text-centered mt-2"
}, "Copyright \xA9 2021-", /*#__PURE__*/React.createElement("span", {
  id: "copyright-end-year"
}), " Elmish contributors."), /*#__PURE__*/React.createElement(CopyrightScript, null));