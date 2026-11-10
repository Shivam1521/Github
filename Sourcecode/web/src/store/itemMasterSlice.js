import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import ItemMasterService from "../services/ItemMasterService";

const service = new ItemMasterService();
export const updateItem = createAsyncThunk(
  "item/updateItem",
  async (payload, { rejectWithValue }) => {
    try {
      const res = await service.UpdateItem(payload);

      if (res.responseCode !== "200") {
        return rejectWithValue(res.responseMessage || "Unknown error");
      }

      return res.responseMessage;
    } catch (err) {
      return rejectWithValue(err.message || "Something went wrong");
    }
  },
);

export const fetchItems = createAsyncThunk(
  "item/fetchItems",
  async (request, { rejectWithValue }) => {
    try {
      const res = await service.GetItems(request);

      if (res.responseCode !== "200") {
        return rejectWithValue(res.responseMessage || "Failed to fetch items");
      }

      return res.responseData || [];
    } catch (err) {
      return rejectWithValue(err.message || "Something went wrong");
    }
  },
);

export const uploadItemMapping = createAsyncThunk(
  "item/uploadItemMapping",
  async (file, { rejectWithValue }) => {
    try {
      const res = await service.UploadItemMapping(file);

      if (res.responseCode !== "200") {
        return rejectWithValue(res.responseMessage);
      }

      return res.responseMessage;
    } catch (err) {
      return rejectWithValue(err.message || "Upload failed");
    }
  }
);

const itemSlice = createSlice({
  name: "item",
  initialState: {
    datasource: [],
    totalRecords: 0,
    loading: false,
    error: null,
    submitLoading: false,
    updateItemError: null,
    updateItemSuccess: "",
    uploadLoading: false,
uploadSuccess: "",
uploadError: null,
  },
  reducers: {
    reset: (state) => {
  state.updateItemError = null;
  state.updateItemSuccess = "";

  state.uploadError = null;
  state.uploadSuccess = "";
},
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchItems.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchItems.fulfilled, (state, action) => {
        state.loading = false;
        state.datasource = action.payload;
        state.totalRecords = action.payload.length
          ? action.payload[0].totalRows
          : 0;
      })
      .addCase(fetchItems.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
        state.datasource = [];
        state.totalRecords = 0;
      })
      .addCase(updateItem.pending, (state, action) => {
        state.submitLoading = true;
        state.updateItemError = null;
      })
      .addCase(updateItem.fulfilled, (state, action) => {
        state.submitLoading = false;
        state.updateItemSuccess = action.payload;
      })
      .addCase(updateItem.rejected, (state, action) => {
        state.submitLoading = false;
        state.updateItemError = action.payload;
      })
      .addCase(uploadItemMapping.pending, (state) => {
    state.uploadLoading = true;
    state.uploadError = null;
})
.addCase(uploadItemMapping.fulfilled, (state, action) => {
    state.uploadLoading = false;
    state.uploadSuccess = action.payload;
})
.addCase(uploadItemMapping.rejected, (state, action) => {
    state.uploadLoading = false;
    state.uploadError = action.payload;
});
  },
});
export const { reset } = itemSlice.actions;
export default itemSlice.reducer;
