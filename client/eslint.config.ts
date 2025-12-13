import globals from "globals";
import tseslint from "typescript-eslint";
import react from "eslint-plugin-react";
import reactHooks from "eslint-plugin-react-hooks";
import {defineConfig} from "eslint/config";
import prettier from "eslint-config-prettier";
import unusedImports from "eslint-plugin-unused-imports";

export default defineConfig([
    // Base config
    {
        files: ["**/*.{js,mjs,cjs,ts,mts,cts,jsx,tsx}"],
        languageOptions: {
            parser: tseslint.parser,
            parserOptions: {
                ecmaVersion: "latest",
                sourceType: "module",
                ecmaFeatures: {jsx: true},
            },
            globals: globals.browser,
        },
        plugins: {
            "@typescript-eslint": tseslint.plugin,
            react,
            "react-hooks": reactHooks,
            "unused-imports": unusedImports,
        },
        settings: {
            react: {version: "detect"},
        },
    },

    // Recommended configs
    tseslint.configs.recommended,
    react.configs.flat.recommended,

    // 🔥 OVERRIDES MUST COME LAST
    {
        rules: {
            /* React 17+ */
            "react/react-in-jsx-scope": "off",

            /* TypeScript */
            "@typescript-eslint/no-unused-vars": [
                "warn",
                {
                    argsIgnorePattern: "^_",
                    varsIgnorePattern: "^_",
                },
            ],

            /* Hooks */
            "react-hooks/rules-of-hooks": "error",
            "react-hooks/exhaustive-deps": "warn",
            "unused-imports/no-unused-imports": "error",
            "unused-imports/no-unused-vars": [
                "warn",
                {varsIgnorePattern: "^_", argsIgnorePattern: "^_"},
            ],
        },
    },
    prettier
]);
