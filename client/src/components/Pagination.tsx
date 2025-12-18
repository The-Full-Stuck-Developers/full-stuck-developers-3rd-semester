import React from "react";
import {useTranslation} from "react-i18next";

interface PaginationProps {
    currentPage: number;
    totalPages: number;
    onPageChange: (page: number) => void;
    perPage: number;
    totalItems: number;
}

const Pagination: React.FC<PaginationProps> = ({
                                                   currentPage,
                                                   totalPages,
                                                   onPageChange,
                                                   perPage,
                                                   totalItems
                                               }) => {
    const {t} = useTranslation();

    const pages: number[] = [];

    const startPage = Math.max(1, currentPage - 2);
    const endPage = Math.min(totalPages, currentPage + 2);

    const startItem = (currentPage - 1) * perPage + 1;
    const endItem = Math.min(currentPage * perPage, totalItems);

    for (let i = startPage; i <= endPage; i++) {
        pages.push(i);
    }

    const handlePrev = () => {
        if (currentPage > 1) onPageChange(currentPage - 1);
    };

    const handleNext = () => {
        if (currentPage < totalPages) onPageChange(currentPage + 1);
    };
    console.log(currentPage, totalPages)
    return (
        <div className={"flex flex-row items-center justify-between w-full"}>
            <div className={"text-white"}>
                Showing {startItem} to {endItem} of {totalItems} results
            </div>

            {totalPages > 1 && (
                <div className="flex h-full text-white">
                    <button
                        className={`${currentPage === 1 ? "disabled" : "block"} w-20 bg-gray-800 hover:bg-gray-700 text-white border border-gray-700 rounded-s-lg transition-colors cursor-pointer disabled:cursor-not-allowed disabled:opacity-50`}
                        onClick={handlePrev}
                        disabled={currentPage === 1}
                    >
                        {t("prev")}
                    </button>

                    {startPage > 1 && (
                        <>
                            <button
                                className="size-10 bg-gray-800 hover:bg-gray-700 text-white cursor-pointer border border-gray-700 transition-colors"
                                onClick={() => onPageChange(1)}
                            >
                                1
                            </button>
                            {startPage > 2 && (
                                <span
                                    className="size-10 text-center align-middle cursor-default bg-gray-900 border border-gray-700 flex items-center justify-center text-gray-400">
              ...
            </span>
                            )}
                        </>
                    )}

                    {pages.map((page) => (
                        <button
                            key={page}
                            className={`size-10 border transition-colors ${
                                page === currentPage
                                    ? "bg-red-600 text-white cursor-default border-red-500"
                                    : "bg-gray-800 hover:bg-gray-700 text-white cursor-pointer border-gray-700"
                            }`}
                            onClick={() => onPageChange(page)}
                            disabled={page === currentPage}
                        >
                            {page}
                        </button>
                    ))}

                    {endPage < totalPages && (
                        <>
                            {endPage < totalPages - 1 && (
                                <span
                                    className="size-10 text-center align-middle cursor-default bg-gray-900 border border-gray-700 flex items-center justify-center text-gray-400">
              ...
            </span>
                            )}
                            <button
                                className="size-10 bg-gray-800 hover:bg-gray-700 text-white cursor-pointer border border-gray-700 transition-colors"
                                onClick={() => onPageChange(totalPages)}
                            >
                                {totalPages}
                            </button>
                        </>
                    )}

                    <button
                        className={`${currentPage === totalPages ? "disabled" : "block"} w-20 bg-gray-800 hover:bg-gray-700 text-white border border-gray-700 rounded-e-lg transition-colors cursor-pointer disabled:cursor-not-allowed disabled:opacity-50`}
                        onClick={handleNext}
                        disabled={currentPage === totalPages}
                    >
                        {t("next")}
                    </button>
                </div>
            )}
        </div>
    );
};

export default Pagination;
