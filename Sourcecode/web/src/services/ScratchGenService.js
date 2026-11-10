import { authHeader, handleResponse } from "../helper/auth-header";

export default class ScratchGenService {
  async GetScratchFile(soNo) {
    const requestOptions = {
      method: "GET",
      headers: authHeader(),
    };

    const url = `${import.meta.env.VITE_API_URL}/Scratch/GetScratchFile?SONo=${soNo}`;
    const response = await fetch(url, requestOptions);

    const contentType = response.headers.get("content-type") || "";
    const isZip =
      contentType.includes("application/zip") ||
      contentType.includes("application/octet-stream");

    if (response.ok && isZip) {
      const blob = await response.blob();
      const contentDisposition = response.headers.get("content-disposition");
      const filename =
        this.getFilenameFromContentDisposition(contentDisposition) ||
        `scratch_${soNo}.zip`;

      return { blob, filename };
    }

    const data = await handleResponse(response, { url, ...requestOptions });
    return data;
  }

  getFilenameFromContentDisposition(contentDisposition) {
    if (!contentDisposition) return null;
    const filenameMatch = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/.exec(
      contentDisposition
    );
    if (!filenameMatch || filenameMatch.length < 2) return null;
    return filenameMatch[1].replace(/['"]/g, "");
  }
  async InsertScratchDetails(obj) {
    const requestOptions = {
      method: "POST",
      headers: authHeader(),
      body: JSON.stringify(obj),
    };

    const url = `${import.meta.env.VITE_API_URL}/Scratch/InsertScratchDetails`;
    const response = await fetch(url, requestOptions);
    const data = await handleResponse(response, { url, ...requestOptions });
    return data;
  }

  async GetScratchDetails(obj) {
    const requestOptions = {
      method: "POST",
      headers: authHeader(),
      body: JSON.stringify(obj),
    };
    const url = `${import.meta.env.VITE_API_URL}/Scratch/GetScratchDetails`;
    const response = await fetch(url, requestOptions);
    const data = await handleResponse(response, { url, ...requestOptions });
    return data;
  }
}
