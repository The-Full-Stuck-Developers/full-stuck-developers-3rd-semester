import { TransactionsClient } from "@core/generated-client.ts";
import { baseUrl } from "@core/baseUrl.ts";
import { TOKEN_KEY, tokenStorage } from "../../atoms/token.ts";

const getTransactionsClient = () => {
  return new TransactionsClient(baseUrl, {
    fetch: async (url, init) => {
      const token = tokenStorage.getItem(TOKEN_KEY, null);

      init = init ?? {};
      init.headers = {
        ...(init.headers ?? {}),
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
      };

      return fetch(url, init);
    },
  });
};

export default getTransactionsClient;
