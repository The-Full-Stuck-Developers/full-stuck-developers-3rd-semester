export async function getProblemTitle(err: any): Promise<string | null> {
    if (typeof err?.result?.title === "string") return err.result.title;
    if (typeof err?.result?.Title === "string") return err.result.Title;

    if (typeof err?.response === "string") {
        try {
            const data = JSON.parse(err.response);
            if (typeof data?.title === "string") return data.title;
        } catch {
            if (err.response.trim()) return err.response;
        }
    }

    const res: Response | undefined = err?.response_ ?? err?.response;
    if (res && typeof res.clone === "function") {
        try {
            const data = await res.clone().json();
            if (typeof data?.title === "string") return data.title;
        } catch {}
    }

    if (typeof err?.message === "string") return err.message;

    return null;
}
