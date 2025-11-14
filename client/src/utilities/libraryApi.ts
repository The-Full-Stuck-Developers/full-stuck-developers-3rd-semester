import {LibraryClient, type Book} from "@core/generated-client.ts";
import {baseUrl} from "@core/baseUrl.ts";
import {customFetch} from "@utilities/customFetch.ts";
import {resolveRefs} from "dotnet-json-refs";

class LibraryClientWithResolvedRefs extends LibraryClient {
    override async getBooks(requestDto: any): Promise<Book[]> {
        const result = await super.getBooks(requestDto);
        return resolveRefs(result);
    }
}

export const libraryApi = new LibraryClientWithResolvedRefs(baseUrl, customFetch);