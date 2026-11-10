import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import ScratchGenService from "../services/ScratchGenService";

const service = new ScratchGenService();
export const saveScratchDetails = createAsyncThunk(
  "scratchGen/saveScratchDetails",
  async (payload, { rejectWithValue }) => {
    try {
      const res = await service.InsertScratchDetails(payload);

      if (res.responseCode !== "200") {
        return rejectWithValue(res.responseMessage || "Unknown error");
      }

      return res.responseMessage;
    } catch (err) {
      return rejectWithValue(err.message || "Something went wrong");
    }
  }
);

export const fetchScratchDetails = createAsyncThunk(
  "scratchGen/fetchScratchDetails",
  async (request, { rejectWithValue }) => {
    try {
      const res = await service.GetScratchDetails(request);
      if (res.responseCode !== "200") {
        return rejectWithValue(
          res.responseMessage || "Failed to fetch scratch details"
        );
      }

      return res.responseData || [];
    } catch (err) {
      return rejectWithValue(err.message || "Something went wrong");
    }
  }
);

export const fetchScratchFile = createAsyncThunk(
  "scratchGen/fetchScratchFile",
  async (request, { rejectWithValue }) => {
    try {
      const res = await service.GetScratchFile(request);

      if (res.responseCode !== "200") {
        return rejectWithValue(res.responseMessage || "Failed");
      }

      return res.fileUrl;     // <-- CHANGE THIS
    } catch (err) {
      return rejectWithValue(err.message || "Something went wrong");
    }
  }
);
// export const fetchScratchFile = createAsyncThunk(
//   "scratchGen/fetchScratchFile",
//   async (request, { rejectWithValue }) => {
//     try {
//       const res = await service.GetScratchFile(request);

//       if (res.responseCode !== "200") {
//         return rejectWithValue(res.responseMessage || "Failed");
//       }

//       return res.data; // ✅ THIS IS THE FIX
//     } catch (err) {
//       return rejectWithValue(err.message || "Something went wrong");
//     }
//   }
// );

const scratchGenSlice = createSlice({
  name: "scratchGen",
  initialState: {
    datasource: [],
    totalRecords: 0,
    loading: false,
    error: null,
    fileError: null,
    fileUrl: "",
    fileLoading: false,
    submitLoading: false,
    createScratchGenError: null,
    createScratchGenSuccess: "",
  },
  reducers: {
    reset: (state) => {
      state.createScratchGenError = null;
      state.createScratchGenSuccess = "";
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchScratchDetails.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchScratchDetails.fulfilled, (state, action) => {
        state.loading = false;
        state.datasource = action.payload;
        state.totalRecords = action.payload.length
          ? action.payload[0].totalRows
          : 0;
      })
      .addCase(fetchScratchDetails.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
      })
      .addCase(fetchScratchFile.pending, (state) => {
        state.fileLoading = true;
        state.fileError = null;
      })
      .addCase(fetchScratchFile.fulfilled, (state, action) => {
        state.fileLoading = false;
        state.fileUrl = action.payload;
      })
      .addCase(fetchScratchFile.rejected, (state, action) => {
        state.fileLoading = false;
        state.fileError = action.payload;
      })
      .addCase(saveScratchDetails.pending, (state, action) => {
        state.submitLoading = true;
        state.createScratchGenError = null;
      })
      .addCase(saveScratchDetails.fulfilled, (state, action) => {
        state.submitLoading = false;
        state.createScratchGenSuccess = action.payload;
      })
      .addCase(saveScratchDetails.rejected, (state, action) => {
        state.submitLoading = false;
        state.createScratchGenError = action.payload;
      });
  },
});
export const { reset } = scratchGenSlice.actions;
export default scratchGenSlice.reducer;
