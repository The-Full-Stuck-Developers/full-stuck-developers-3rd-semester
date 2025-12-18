import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import enCommon from "./locales/en/common.json";
import enMessages from "./locales/en/messages.json";
import enPlaceholders from "./locales/en/placeholders.json";
import enUser from "./locales/en/user.json";
import enValidation from "./locales/en/validation.json";
import enGame from "./locales/en/game.json";
import enPlayer from "./locales/en/player.json";
import enHome from "./locales/en/home.json";

import dkCommon from "./locales/dk/common.json";
import dkMessages from "./locales/dk/messages.json";
import dkPlaceholders from "./locales/dk/placeholders.json";
import dkUser from "./locales/dk/user.json";
import dkValidation from "./locales/dk/validation.json";
import dkGame from "./locales/dk/game.json";
import dkPlayer from "./locales/dk/player.json";
import dkHome from "./locales/dk/home.json";

i18n.use(initReactI18next).init({
  resources: {
    en: {
      common: enCommon,
      messages: enMessages,
      placeholders: enPlaceholders,
      user: enUser,
      validation: enValidation,
      game: enGame,
      player: enPlayer,
      home: enHome,
    },
    dk: {
      common: dkCommon,
      messages: dkMessages,
      placeholders: dkPlaceholders,
      user: dkUser,
      validation: dkValidation,
      game: dkGame,
      player: dkPlayer,
      home: dkHome,
    },
  },
  lng: "en",
  fallbackLng: "en",
  ns: [
    "common",
    "messages",
    "placeholders",
    "user",
    "validation",
    "game",
    "player",
  ],
  defaultNS: "common",
  interpolation: {
    escapeValue: false,
  },
});

export default i18n;
