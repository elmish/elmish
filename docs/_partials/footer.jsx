import React from 'react';

const SitemapSection = ({ title, children }) => (
    <div className="sitemap-section">
        <div className="sitemap-section-title">
            {title}
        </div>
        <ul className="sitemap-section-list">
            {children}
        </ul>
    </div>
)

const SitemapSectionItem = ({ text, icon, url }) => (
    <li>
        <a href={url} className="icon-text sitemap-section-list-item">
            <span className="icon">
                <i className={icon}></i>
            </span>
            <span className="sitemap-section-list-item-text">
                {text}
            </span>
        </a>
    </li>
)

const CopyrightScript = () => (
    <script dangerouslySetInnerHTML={{
        __html: `
        const year = new Date().getFullYear();
        document.getElementById('copyright-end-year').innerHTML = year;
        `
    }} />
)

export default (
    <div className="is-size-5">
        <div className="sitemap">
            <SitemapSection title="Project ressources">
                <SitemapSectionItem
                    text="Repository"
                    icon="fas fa-file-code"
                    url="https://github.com/elmish/elmish" />

                <SitemapSectionItem
                    text="Release notes"
                    icon="fas fa-list"
                    url="/elmish/release_notes.html" />

                <SitemapSectionItem
                    text="License"
                    icon="fas fa-id-card"
                    url="https://github.com/elmish/elmish/blob/v4.x/LICENSE.md" />
            </SitemapSection>
            <SitemapSection title="Elmish modules">
                <SitemapSectionItem
                    text="Fable.Elmish"
                    icon="fa fa-book"
                    url="https://elmish.github.io/elmish/" />

                <SitemapSectionItem
                    text="Fable.Elmish.Browser"
                    icon="fa fa-book"
                    url="https://elmish.github.io/browser/" />

                <SitemapSectionItem
                    text="Fable.Elmish.UrlParser"
                    icon="fa fa-book"
                    url="https://elmish.github.io/urlParser/" />

                <SitemapSectionItem
                    text="Fable.Elmish.Debugger"
                    icon="fa fa-book"
                    url="https://elmish.github.io/debugger/" />

                <SitemapSectionItem
                    text="Fable.Elmish.React"
                    icon="fa fa-book"
                    url="https://elmish.github.io/react/" />

                <SitemapSectionItem
                    text="Fable.Elmish.HMR"
                    icon="fa fa-book"
                    url="https://elmish.github.io/hmr/" />

            </SitemapSection>
            <SitemapSection title="Other Links">
                <SitemapSectionItem
                    text="Fable"
                    icon="faf faf-fable"
                    url="https://fable.io" />

                <SitemapSectionItem
                    text="Fable Gitter"
                    icon="fab fa-gitter"
                    url="https://gitter.im/fable-compiler/Fable" />

                <SitemapSectionItem
                    text="F# Slack"
                    icon="fab fa-slack"
                    url="https://fsharp.org/guides/slack/" />

                <SitemapSectionItem
                    text="F# Software Foundation"
                    icon="faf faf-fsharp-org"
                    url="https://fsharp.org/" />
            </SitemapSection>

        </div>
        <p className="has-text-centered">
            Built with <a className="is-underlined" href="https://mangelmaxime.github.io/Nacara/">Nacara</a>
        </p>
        <p className="has-text-centered mt-2">
            Copyright © 2021-<span id="copyright-end-year"/> Elmish contributors.
        </p>
        <CopyrightScript />
    </div>
)
